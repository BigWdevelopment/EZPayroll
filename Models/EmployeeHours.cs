using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace EZPayroll.MVC.Models
{
    public class EmployeeHours
    {
        [Key]
        public int EmpHoursId { get; set; }
        [Required]
        public int EmpId { get; set; }

        [Required]
        public int TaskId { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime Date { get; set; }
        [Required]
        [Column(TypeName = "decimal(18,4)")]
        public Decimal Rate { get; set; }
        [Required]
        [DisplayFormat(DataFormatString = "{0:#.##}")]
        [Column(TypeName = "decimal(18,2)")]
        public Decimal Total { get; set; }
        [Required]
        public int Hours { get; set; }
    }
}
