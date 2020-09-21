using EZPayroll.MVC.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EZPayroll.MVC.ViewModels
{
    public class AssignmentsViewModel
    {

        //Empty Constructor
        public AssignmentsViewModel()
        {

        }

        //DB Bound Constructor
        private readonly SystemDbContext _context;
        public AssignmentsViewModel(SystemDbContext context)
        {
            _context = context;
        }
        public Assignments Assignee { get; set; }

        public string _TaskName { get; set; }
        public string _EmployeeName { get; set; }
        public int _AssignmentId { get; set; }
        public SelectList Tasks(int? id)
        {
            try
            {
                var listFromDB = _context.Tasks.ToList();
                List<SelectListItem> ListToAddTo = new List<SelectListItem>();

                foreach (var item in listFromDB)
                {
                    if (id != 0 && id == item.TaskId)
                    {
                        ListToAddTo.Add(new SelectListItem { Value = item.TaskId.ToString(), Text = item.TaskName, Selected = true });
                    }
                    else
                    {
                        ListToAddTo.Add(new SelectListItem { Value = item.TaskId.ToString(), Text = item.TaskName });
                    }
                }

                return new SelectList(ListToAddTo, "Value", "Text", id);
            }

            catch (Exception)
            {
                //Log Error
            }

            return new SelectList(null);

        }

        public string TasksName(int id)
        {
            try
            {
                var listFromDB = _context.Tasks.Where(d=>d.TaskId == id).ToList().FirstOrDefault();

                return listFromDB.TaskName;
            }

            catch (Exception)
            {
                //Log Error
            }

            return "";

        }

        public string EmployeeName(int id)
        {
            try
            {
                var listFromDB = _context.Employees.Where(d => d.EmpId == id).ToList().FirstOrDefault();

                return listFromDB.Name + " " + listFromDB.Surname;
            }
            catch (Exception)
            {
                //Log Error
            }

            return "";

        }
        public SelectList Employees(int? id)
        {
            try
            {
                var listFromDB = _context.Employees.ToList();
                List<SelectListItem> ListToAddTo = new List<SelectListItem>();

                foreach (var item in listFromDB)
                {
                    if (id != 0 && id == item.EmpId)
                    {
                        ListToAddTo.Add(new SelectListItem { Value = item.EmpId.ToString(), Text = item.Name + " " + item.Surname, Selected = true });
                    }
                    else
                    {
                        ListToAddTo.Add(new SelectListItem { Value = item.EmpId.ToString(), Text = item.Name + " " + item.Surname });
                    }

                }



                return new SelectList(ListToAddTo, "Value", "Text", id);
            }

            catch (Exception)
            {
                //Log Error
            }

            return new SelectList(null);

        }

        public static Assignments ConvertViewModel(AssignmentsViewModel vm)
        {
            if (vm.Assignee.AssignmentId != 0)
            {
                return new Assignments
                {
                    AssignmentId = vm.Assignee.AssignmentId,
                    EmpId = vm.Assignee.EmpId,
                    TaskId = vm.Assignee.TaskId
                };
            }
            else
            {
                return new Assignments
                {
                    EmpId = vm.Assignee.EmpId,
                    TaskId = vm.Assignee.TaskId
                };
            }

        }
        public static List<AssignmentsViewModel> ConvertModelToVM( SystemDbContext context, List<Assignments> assign)
        {
            List<AssignmentsViewModel> list = new List<AssignmentsViewModel>();
            foreach (var item in assign)
            {
                AssignmentsViewModel assignment = new AssignmentsViewModel(context);
                assignment._EmployeeName = assignment.EmployeeName(item.EmpId);
                assignment._TaskName = assignment.TasksName(item.TaskId);
                assignment._AssignmentId = item.AssignmentId;
                list.Add(assignment);
            }
            return list;

        }


    }
}

