using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EZPayroll.MVC.Models
{
    public class Assignments
    {
        [Key] 
        public int AssignmentId { get; set; }
        public int EmpId { get; set; }

        public int TaskId { get; set; }
    }
}
