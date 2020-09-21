using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EZPayroll.MVC.Models;
using EZPayroll.MVC.ViewModels;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using System.Net.Http;

namespace EZPayroll.MVC.Controllers
{
    public class EmployeesController : Controller
    {
        private readonly SystemDbContext _context;
        private readonly IWebHostEnvironment _env;

        public EmployeesController(SystemDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        // GET: Employees
        public  IActionResult Index()
        {
            IEnumerable<EmployeesViewModel> list = null;
            try
            {
               list = EmployeesViewModel.ConvertModelToVM(_context.Employees.ToList(), _context);
                if (list == null)
                {
                    return NotFound();
                }               
            }
            catch (Exception)
            {
                list = Enumerable.Empty<EmployeesViewModel>();

                ModelState.AddModelError(string.Empty, "Server Error. Please contact administrator.");
               
            }
            return View(list);

        }

    

        // GET: Employees/Create
        public IActionResult Create()
        {
            try
            {
                if (_context.Roles.ToList().Count < 1)
                {
                    ModelState.AddModelError(string.Empty, "Please create a role first.");
                }
            }
            catch (Exception)
            {
                ModelState.AddModelError(string.Empty, "Server Error. Please contact administrator.");
                IEnumerable<EmployeesViewModel> list = null; 
                return View(list);
            }
          
      
            return View(new EmployeesViewModel(_context));
        }

        // POST: Employees/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(EmployeesViewModel employees)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (employees.ProfileImage != null)
                    {
                        if (employees.ProfileImage.Length > 0)
                        {
                            using (var oStream = employees.ProfileImage.OpenReadStream())
                            using (var mStream = new MemoryStream())
                            {
                                oStream.CopyTo(mStream);
                                employees.PicArray = mStream.ToArray();
                            }
                        }
                    }
                }
                catch (Exception)
                {
                    //Log error
                }              
                
                _context.Add(EmployeesViewModel.ConvertViewModel(employees));
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(employees);
        }      
    

    // GET: Employees/Edit/5
    public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employees = await _context.Employees.FindAsync(id);
            if (employees == null)
            {
                return NotFound();
            }
            return View(new EmployeesViewModel(_context) {Employee = employees });
        }

        // POST: Employees/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EmployeesViewModel employees)
        {
          
            if (ModelState.IsValid)
            {
                try
                {
                    if (employees.ProfileImage != null)
                    {                        
                            if (employees.ProfileImage.Length > 0)
                            {
                                using (var oStream = employees.ProfileImage.OpenReadStream())
                                using (var mStream = new MemoryStream())
                                {
                                    oStream.CopyTo(mStream);
                                    employees.PicArray = mStream.ToArray();
                                }
                            }
                        
                    }              
                    _context.Update(EmployeesViewModel.ConvertViewModel(employees));
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EmployeesExists(employees.Employee.EmpId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        //Log error
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(employees);
        }


        // POST: Employees/Delete/5
        // POST: Roles/Delete/5
        [HttpDelete]
        public IActionResult Delete(int id)
        {
            if (id < 1)
            {
                return Json(new { success = false, message = "Error while Deleting" });
            }
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(LocalURI());

                //HTTP DELETE
                var deleteTask = client.DeleteAsync("Employees/" + id.ToString());
                deleteTask.Wait();

                var result = deleteTask.Result;
                if (result.IsSuccessStatusCode)
                {

                    return Json(new { success = true, message = result.Content });
                }

            }

            return Json(new { success = false, message = "Error while Deleting" });
        }
        private string LocalURI()
        {
            var baseUrl = $"{this.Request.Scheme}://{this.Request.Host.Value.ToString()}{this.Request.PathBase.Value.ToString()}";

            return baseUrl.ToString() + "/api/";
        }

        private bool EmployeesExists(int id)
        {
            return _context.Employees.Any(e => e.EmpId == id);
        }
    }
}
