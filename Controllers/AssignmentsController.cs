using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EZPayroll.MVC.Models;
using EZPayroll.MVC.ViewModels;
using System.Net.Http;

namespace EZPayroll.MVC.Controllers
{
    public class AssignmentsController : Controller
    {
        private readonly SystemDbContext _context;

        public AssignmentsController(SystemDbContext context)
        {
            _context = context;
        }

        // GET: Assignments
        public  IActionResult Index()
        {
          
            IEnumerable<AssignmentsViewModel> list = null;
            try
            {
                list = AssignmentsViewModel.ConvertModelToVM(_context, _context.Assignments.ToList());
                if (list == null)
                {
                    return NotFound();
                }
            }
            catch (Exception)
            {
                list = Enumerable.Empty<AssignmentsViewModel>();

                ModelState.AddModelError(string.Empty, "Server Error. Please contact administrator.");

            }
            return View(list);
        }

        // GET: Assignments/Create
        public IActionResult Create(int EmpId)
        {
            try
            {
                var items = _context.Assignments.Count();
                var item = new AssignmentsViewModel(_context);
                return View(item);
            }
            catch (Exception)
            {
                var item = new AssignmentsViewModel();
                ModelState.AddModelError(string.Empty, "Server Error. Please contact administrator."); return View(item = null);
            }
           
        }

        // POST: Assignments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AssignmentsViewModel assignments)
        {
            try
            {                
                if ( _context.Assignments.Where(d => d.EmpId == assignments.Assignee.EmpId && d.TaskId == assignments.Assignee.TaskId).ToListAsync().Result.Count > 0)
                {
                    ModelState.AddModelError(string.Empty, "Error, Duplicate assignment.");
                }
            }
            catch (Exception)
            {
//Log error
            }
           
            if (ModelState.IsValid)
            {
                _context.Add(AssignmentsViewModel.ConvertViewModel(assignments));
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            else
            {
                return View(new AssignmentsViewModel(_context));
            }
          
        }
              

        // GET: Assignments/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var assignments = await _context.Assignments
                .FirstOrDefaultAsync(m => m.AssignmentId == id);
            if (assignments == null)
            {
                return NotFound();
            }

            return View(assignments);
        }

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
                var deleteTask = client.DeleteAsync("Assignments/" + id.ToString());
                deleteTask.Wait();

                var result = deleteTask.Result;
                if (result.IsSuccessStatusCode)
                {

                    return Json(new { success = true, message = result.Content });
                }

            }

            return Json(new { success = false, message = "Error while Deleting" });
        }

        private bool AssignmentsExists(int id)
        {
            return _context.Assignments.Any(e => e.AssignmentId == id);
        }
        private string LocalURI()
        {
            var baseUrl = $"{this.Request.Scheme}://{this.Request.Host.Value.ToString()}{this.Request.PathBase.Value.ToString()}";

            return baseUrl.ToString() + "/api/";
        }
    }
}
