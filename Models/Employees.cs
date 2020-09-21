using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EZPayroll.MVC.Models
{
    public class Employees
    {
        [Key]
        public int EmpId { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Surname { get; set; }

        [MaxLength]
        public byte[] DataFiles { get; set; }

        [Required]
        public int RoleId { get; set; }

    }
}
