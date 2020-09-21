using EZPayroll.MVC.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EZPayroll.MVC.ViewModels
{
    public class EmployeesViewModel
    {
        //Empty Constructor
        public EmployeesViewModel()
        {

        }

        //DB Bound Constructor
        private readonly SystemDbContext _context;
        public EmployeesViewModel(SystemDbContext context)
        {
            _context = context;
        }
        public Employees Employee { get; set; }
        public string RoleName { get; set; }

        public int TempEmpId { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }       

        [Display(Name = "Profile Picture")]
        public IFormFile ProfileImage { get; set; }

        public byte[] PicArray { get; set; }
        public SelectList Roles(int? id)
        {
            try
            {
                var listFromDB = _context.Roles.ToList();
                List<SelectListItem> ListToAddTo = new List<SelectListItem>();
               
                    foreach (var item in listFromDB)
                    {
                        if (id != 0 && id == item.RoleId)
                        {
                            ListToAddTo.Add(new SelectListItem { Value = item.RoleId.ToString(), Text = item.RoleName, Selected = true });
                        }
                        else
                        {
                            ListToAddTo.Add(new SelectListItem { Value = item.RoleId.ToString(), Text = item.RoleName });
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
        public string GetRoleName(int id)
        {
            try
            {
                var listFromDB = _context.Roles.Where(d=>d.RoleId == id).ToList().FirstOrDefault();
                List<SelectListItem> ListToAddTo = new List<SelectListItem>();

                return listFromDB.RoleName;
            }

            catch (Exception)
            {
                //Log Error
            }

            return "";

        }

        public static Employees ConvertViewModel(EmployeesViewModel vm)
        {
            try
            {
                if (vm.Employee.EmpId != 0)
                {
                    return new Employees
                    {
                        EmpId = vm.Employee.EmpId,
                        Name = vm.Employee.Name,
                        Surname = vm.Employee.Surname,
                        DataFiles = vm.PicArray,
                        RoleId = vm.Employee.RoleId
                    };
                }
                else
                {
                    return new Employees
                    {
                        Name = vm.Employee.Name,
                        Surname = vm.Employee.Surname,
                        DataFiles = vm.PicArray,
                        RoleId = vm.Employee.RoleId
                    };
                }
            }
            catch (Exception)
            {
             //Log error
             return null;
            }
             
        }

        public static List<EmployeesViewModel> ConvertModelToVM(List<Employees> vm, SystemDbContext context)
        {
            try
            {
                List<EmployeesViewModel> list = new List<EmployeesViewModel>();
                foreach (var item in vm)
                {
                    EmployeesViewModel emp = new EmployeesViewModel(context);
                    emp.TempEmpId = item.EmpId;
                    emp.Name = item.Name;
                    emp.Surname = item.Surname;
                    emp.PicArray = item.DataFiles;
                    emp.RoleName = emp.GetRoleName(item.RoleId);
                    list.Add(emp);
                }
                return list;
            }
            catch (Exception)
            {
                //Log error
                return null;
            }
                     

        }


    }
}
