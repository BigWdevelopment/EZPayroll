using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EZPayroll.MVC.Models;
using System.Net.Http;

namespace EZPayroll.MVC.Controllers
{
    public class TasksController : Controller
    {
        private readonly SystemDbContext _context;

        public TasksController(SystemDbContext context)
        {
            _context = context;
        }

        // GET: Tasks
        public async Task<IActionResult> Index()
        {
            IEnumerable<Tasks> list = null;
            try
            {
                list = await _context.Tasks.ToListAsync();
            }
            catch (Exception)
            {

                ModelState.AddModelError(string.Empty, "Server Error. Please contact administrator.");
                list = Enumerable.Empty<Tasks>();
            }
            return View(list);
        }

        // GET: Tasks/Details/5
      
        // GET: Tasks/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Tasks/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("TaskId,TaskName,EndDate,StartDate")] Tasks tasks)
        {
            if (tasks.StartDate > tasks.EndDate || tasks.EndDate < tasks.StartDate)
            {
                ModelState.AddModelError(string.Empty, "Please select a correct date range.");
            }
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Add(tasks);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception)
                {
                    ModelState.AddModelError(string.Empty, "Server Error. Please contact administrator.");
                }              
            }           
            return View(tasks);
        }

        // GET: Tasks/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tasks = await _context.Tasks.FindAsync(id);
            if (tasks == null)
            {
                return NotFound();
            }
            return View(tasks);
        }

        // POST: Tasks/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("TaskId,TaskName,EndDate,StartDate")] Tasks tasks)
        {
            if (id != tasks.TaskId)
            {
                return NotFound();
            }
            if (tasks.StartDate > tasks.EndDate || tasks.EndDate < tasks.StartDate)
            {
                ModelState.AddModelError(string.Empty, "Please select a correct date range.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tasks);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TasksExists(tasks.TaskId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(tasks);
        }

        
        // POST: Tasks/Delete/5
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
                var deleteTask = client.DeleteAsync("Tasks/" + id.ToString());
                deleteTask.Wait();

                var result = deleteTask.Result;
                if (result.IsSuccessStatusCode)
                {

                    return Json(new { success = true, message = result.Content });
                }

            }

            return Json(new { success = false, message = "Error while Deleting" });
        }

        private bool TasksExists(int id)
        {
            return _context.Tasks.Any(e => e.TaskId == id);
        }
        private string LocalURI()
        {
            var baseUrl = $"{this.Request.Scheme}://{this.Request.Host.Value.ToString()}{this.Request.PathBase.Value.ToString()}";

            return baseUrl.ToString() + "/api/";
        }
    }
}
