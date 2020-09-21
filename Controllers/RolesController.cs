using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EZPayroll.MVC.Models;
using System.Net.Http;
using Microsoft.AspNetCore.Hosting;

namespace EZPayroll.MVC.Controllers
{
    public class RolesController : Controller
    {

        private readonly SystemDbContext _context;
        public RolesController(SystemDbContext context)
        {
            _context = context;
        }

        // GET: Roles
        public IActionResult Index()
        {
            IEnumerable<Roles> _roles = null;

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(LocalURI());
                //HTTP GET
                var responseTask = client.GetAsync("roles");
                responseTask.Wait();

                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsAsync<IList<Roles>>();
                    readTask.Wait();

                    _roles = readTask.Result;
                }
                else //web api sent error response 
                {
                    _roles = Enumerable.Empty<Roles>();

                    ModelState.AddModelError(string.Empty, "Server Error. Please contact administrator.");
                }
            }
            return View(_roles);
        }

        // GET: Roles/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Roles/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create([Bind("RoleId,RoleName,RoleRate,EndDate")] Roles roles)
        {
            if (roles.RoleRate < 1)
            {
                ModelState.AddModelError(string.Empty, "Please enter a value greater than 0");
            }
            if (ModelState.IsValid)
            {
                using (var client = new HttpClient())
                {

                    client.BaseAddress = new Uri(LocalURI());

                    //HTTP POST
                    var postTask = client.PostAsJsonAsync<Roles>("roles", roles);
                    postTask.Wait();

                    var result = postTask.Result;
                    if (result.IsSuccessStatusCode)
                    {
                        return RedirectToAction("Index");
                    }
                    else //web api sent error response 
                    {
                        ModelState.AddModelError(string.Empty, "API Connection Error. Please contact administrator.");
                    }
                }

                ModelState.AddModelError(string.Empty, "Server Error. Please contact administrator.");
            }
            return View(roles);
        }

        // GET: Roles/Edit/5
        public IActionResult Edit(int? id)
        {
            Roles role = null;

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(LocalURI());
                //HTTP GET
                var responseTask = client.GetAsync("roles/" + id);
                responseTask.Wait();

                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsAsync<Roles>();
                    readTask.Wait();

                    role = readTask.Result;
                }
                else //web api sent error response 
                {
                    ModelState.AddModelError(string.Empty, "Server error. Please contact administrator.");
                }
            }
            return View(role);
        }

        // POST: Roles/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, [Bind("RoleId,RoleName,RoleRate,EndDate")] Roles roles)
        {
            if (id != roles.RoleId)
            {
                return NotFound();
            }
            if (roles.RoleRate < 1)
            {
                ModelState.AddModelError(string.Empty, "Please enter a value greater than 0");
            }

            if (ModelState.IsValid)
            {
                roles.RoleId = id;
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(LocalURI());

                    //HTTP POST
                    var putTask = client.PutAsJsonAsync<Roles>("roles", roles);
                    putTask.Wait();

                    var result = putTask.Result;
                    if (result.IsSuccessStatusCode)
                    {
                        return RedirectToAction("Index");
                    }
                    else //web api sent error response 
                    {
                        ModelState.AddModelError(string.Empty, "Server error. Please contact administrator.");
                    }
                }
            }
            return View(roles);
        }

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
                var deleteTask = client.DeleteAsync("Roles/" + id.ToString());
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
    }
}
