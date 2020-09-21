using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EZPayroll.MVC.Models
{
    public class SystemDbContext : DbContext
    {

        public SystemDbContext(DbContextOptions<SystemDbContext> options)
          : base(options)
        {
        }

        public DbSet<Employees> Employees { get; set; }
        public DbSet<Tasks> Tasks { get; set; }
        public DbSet<Roles> Roles { get; set; }
        public DbSet<Assignments> Assignments { get; set; }
        public DbSet<EmployeeHours> EmployeeHours { get; set; }
    }
}
