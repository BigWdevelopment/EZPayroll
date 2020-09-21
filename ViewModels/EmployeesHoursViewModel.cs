using EZPayroll.MVC.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EZPayroll.MVC.ViewModels
{
    public class EmployeesHoursViewModel
    {

        public string EmployeeName { get; set; }
        public int TempEmpId { get; set; }

        public string TaskName { get; set; }

        public SelectList TaskList { get; set; }

        public EmployeeHours EmployeeHours { get; set; }
        //Empty Constructor
        public EmployeesHoursViewModel()
        {

        }

        //DB Bound Constructor
        private readonly SystemDbContext _context;
        public EmployeesHoursViewModel(SystemDbContext context)
        {
            _context = context;
        }
        public EmployeesHoursViewModel(SystemDbContext context, int EmpId)
        {
            _context = context;
            try
            {
                var _emp = _context.Employees.Find(EmpId);
                EmployeeName = _emp.Name + " " + _emp.Surname;
                TaskList = Tasks(_context.Assignments.Where(d => d.EmpId == EmpId).ToList());
                TempEmpId = EmpId;
            }
            catch (Exception)
            {
                //log
            }

        }

        private SelectList Tasks(List<Assignments> listFromDB)
        {
            try
            {
                var _tasks = _context.Tasks.ToList();
                List<SelectListItem> ListToAddTo = new List<SelectListItem>();

                foreach (var item in listFromDB)
                {
                    var taskListItem = _tasks.Where(d => d.TaskId == item.TaskId).FirstOrDefault();

                    ListToAddTo.Add(new SelectListItem { Value = item.TaskId.ToString(), Text = taskListItem.TaskName });

                }

                return new SelectList(ListToAddTo, "Value", "Text");
            }

            catch (Exception)
            {
                //Log Error
            }

            return new SelectList(null);

        }

        public static EmployeeHours ConvertViewModel(EmployeesHoursViewModel vm)
        {

            if (vm.EmployeeHours.EmpHoursId != 0)
            {
                return new EmployeeHours
                {
                    EmpHoursId = vm.EmployeeHours.EmpHoursId,
                    EmpId = vm.TempEmpId,
                    Date = vm.EmployeeHours.Date,
                    TaskId = vm.EmployeeHours.TaskId,
                    Rate = vm.EmployeeHours.Rate,
                    Hours = vm.EmployeeHours.Hours,
                    Total = vm.EmployeeHours.Total

                };
            }
            else
            {
                return new EmployeeHours
                {
                    EmpId = vm.TempEmpId,
                    Date = vm.EmployeeHours.Date,
                    TaskId = vm.EmployeeHours.TaskId,
                    Rate = vm.EmployeeHours.Rate,
                    Hours = vm.EmployeeHours.Hours,
                    Total = vm.EmployeeHours.Total

                };
            }

        }

    }
}
