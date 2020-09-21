using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace EZPayroll.MVC.Models
{
    public class Roles
    {
        [Key]
        public int RoleId { get; set; }

        [Required]
        public string RoleName { get; set; }
        [Required]
        [Column(TypeName = "decimal(18,4)")]
        public Decimal RoleRate { get; set; }
     
    }
}
