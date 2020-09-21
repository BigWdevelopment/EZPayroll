using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EZPayroll.MVC.Models;
using EZPayroll.MVC.ViewModels;

namespace EZPayroll.MVC.Controllers
{
    public class EmployeeHoursController : Controller
    {
        private readonly SystemDbContext _context;

        public EmployeeHoursController(SystemDbContext context)
        {
            _context = context;
        }

        // GET: EmployeeHours
        public async Task<IActionResult> Index()
        {
            return View(await _context.EmployeeHours.ToListAsync());
        }


        // GET: EmployeeHours/Create
        public IActionResult Create(int id)
        {
            if (_context.Assignments.Where(d=>d.EmpId == id).ToList().Count() < 1)
            {
                ModelState.AddModelError(string.Empty, "Please assign a task to the employee first.");
            }
            return View(new EmployeesHoursViewModel(_context, id));
        }

        // POST: EmployeeHours/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(EmployeesHoursViewModel _employeeHours)
        {
            if (_employeeHours == null)
            {
                return NotFound();
            }
            var currenthours = _context.EmployeeHours.Where(d => d.Date.Date == _employeeHours.EmployeeHours.Date.Date).ToList();
            if (currenthours.Count > 0)
            {
                int totalForDate = _employeeHours.EmployeeHours.Hours;
                foreach (var item in currenthours)
                {
                    totalForDate += item.Hours;
                }
                if (totalForDate > 12)
                {
                    ModelState.AddModelError("Max Hours", "An employee can be assigned multiple tasks but cannot work more than 12 hours a day.");
                    EmployeesHoursViewModel vm = new EmployeesHoursViewModel(_context, _employeeHours.TempEmpId);
                    _employeeHours.TaskList = vm.TaskList;
                }

            }

            //get task dates
            var tasks =  _context.Tasks.Where(d=>d.TaskId == _employeeHours.EmployeeHours.TaskId).FirstOrDefault();
            if (tasks != null)
            {
                DateTime startDate = tasks.StartDate;
                DateTime endDate = tasks.EndDate;

                if (_employeeHours.EmployeeHours.Date < startDate)
                {
                    ModelState.AddModelError("Start Date out of range", "Please select a valid date. The start date for the task selected is " + startDate.ToShortDateString());
                    EmployeesHoursViewModel vm = new EmployeesHoursViewModel(_context, _employeeHours.TempEmpId);
                    _employeeHours.TaskList = vm.TaskList;
                }
                if (_employeeHours.EmployeeHours.Date > endDate)
                {
                    ModelState.AddModelError("End Date out of range", "Please select a valid date. The end date for the task selected is " + endDate.ToShortDateString());
                    EmployeesHoursViewModel vm = new EmployeesHoursViewModel(_context, _employeeHours.TempEmpId);
                    _employeeHours.TaskList = vm.TaskList;
                }
            }

            if (ModelState.IsValid)
            {
                decimal rate = _context.Roles.Where(d => d.RoleId == _context.Employees.Find(_employeeHours.TempEmpId).RoleId).FirstOrDefault().RoleRate;
                int hours = _employeeHours.EmployeeHours.Hours;
                _employeeHours.EmployeeHours.Rate = rate;
                _employeeHours.EmployeeHours.Total = Tools.TotalCalculator.GetCaptureTotal(rate, hours);
                _context.Add(EmployeesHoursViewModel.ConvertViewModel(_employeeHours));
                await _context.SaveChangesAsync();
                return RedirectToAction("Index","Employees");
            }
            return View(_employeeHours);
        }

        // GET: EmployeeHours/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            if (id == 0)
            {
                return NotFound();
            }

            var employeeHours = await _context.EmployeeHours.FindAsync(id);
            if (employeeHours == null)
            {
                return NotFound();
            }
            return View(new EmployeesHoursViewModel(_context, employeeHours.EmpId) { EmployeeHours = employeeHours });
        }

        // POST: EmployeeHours/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EmployeesHoursViewModel _employeeHours)
        {
            if (_employeeHours == null)
            {
                return NotFound();
            }
            var currenthours = _context.EmployeeHours.Where(d => d.Date.Date == _employeeHours.EmployeeHours.Date.Date && d.EmpHoursId != _employeeHours.EmployeeHours.EmpHoursId).ToList();
            if (currenthours.Count > 0)
            {
                int totalForDate = _employeeHours.EmployeeHours.Hours;
                foreach (var item in currenthours)
                {
                    totalForDate += item.Hours;
                }
                if (totalForDate > 12)
                {
                    ModelState.AddModelError("Max Hours", "An employee can be assigned multiple tasks but cannot work more than 12 hours a day.");
                    EmployeesHoursViewModel vm = new EmployeesHoursViewModel(_context, _employeeHours.TempEmpId);
                    _employeeHours.TaskList = vm.TaskList;
                }

            }
            if (ModelState.IsValid)
            {
                try
                {
                    decimal rate = _context.Roles.Where(d => d.RoleId == _context.Employees.Find(_employeeHours.TempEmpId).RoleId).FirstOrDefault().RoleRate;
                    int hours = _employeeHours.EmployeeHours.Hours;
                    _employeeHours.EmployeeHours.Rate = rate;
                    _employeeHours.EmployeeHours.Total = Tools.TotalCalculator.GetCaptureTotal(rate, hours);
                    _context.Update(EmployeesHoursViewModel.ConvertViewModel(_employeeHours));
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EmployeeHoursExists(_employeeHours.EmployeeHours.EmpHoursId))
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
            return View(_employeeHours);
        }

        // GET: EmployeeHours/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employeeHours = await _context.EmployeeHours
                .FirstOrDefaultAsync(m => m.EmpHoursId == id);
            if (employeeHours == null)
            {
                return NotFound();
            }

            return View(employeeHours);
        }

        // POST: EmployeeHours/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var employeeHours = await _context.EmployeeHours.FindAsync(id);
            _context.EmployeeHours.Remove(employeeHours);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EmployeeHoursExists(int id)
        {
            return _context.EmployeeHours.Any(e => e.EmpHoursId == id);
        }
        public IActionResult Search()
        {                       
            try
            {
                var items = _context.Assignments.Count();
                var vm = new EmployeesHoursSearchViewModel(_context);
                return View(vm);
            }
            catch (Exception)
            {             
                ModelState.AddModelError(string.Empty, "Server Error. Please contact administrator."); return View();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Search(int EmpId, DateTime StartDate, DateTime EndDate)
        {

            if (EmpId > 0 && StartDate  != null && EndDate != null)
            {
                var vm = new EmployeesHoursSearchViewModel(_context,EmpId, StartDate, EndDate);
                return View(vm);
            }
            else
            {
                var vm = new EmployeesHoursSearchViewModel(_context);
                return View(vm);
            }

        }
    }
}
