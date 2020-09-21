using EZPayroll.MVC.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EZPayroll.MVC.ViewModels
{
    public class EmployeesHoursSearchViewModel
    {
        //DB Bound Constructor
        private readonly SystemDbContext _context;

        public SelectList EmployeeList { get; set; }

        public int EmpId { get; set; }

        public decimal Total { get; set; }

        public IEnumerable<EmployeeHours> SearchList { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime EndDate { get; set; }
        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime StartDate { get; set; }
        public EmployeesHoursSearchViewModel(SystemDbContext context)
        {
            _context = context;
            EmployeeList = EmployeeSearch();
          
        }
        public EmployeesHoursSearchViewModel(SystemDbContext context,int empId,DateTime start,DateTime end)
        {
            _context = context;
            EmployeeList = EmployeeSearch();
            SearchList = HoursSearch(empId,start,end);
            foreach (var item in SearchList)
            {
                Total += item.Total;
            }
        }
        public SelectList EmployeeSearch()
        {                        
                SelectListItem[] list = _context.Employees.Select(u => new SelectListItem { Value = u.EmpId.ToString(), Text = u.Name + " " + u.Surname }).ToArray();
                return new SelectList(list, "Value", "Text");
           
        }
        public IEnumerable<EmployeeHours> HoursSearch(int _empID, DateTime _start, DateTime _end)
        {
           return _context.EmployeeHours.Where(d=>d.EmpId == _empID && d.Date.Date <= _end.Date && d.Date.Date >= _start.Date).ToList();       

        }

        public string GetTaskName(int _taskId)
        {
            return _context.Tasks.Where(d => d.TaskId == _taskId).FirstOrDefault().TaskName;

        }
    }
}
