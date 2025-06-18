using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManagement.Infrastructures.Identity;
using TaskManagementApi.Domains.Entities;

namespace TaskManagement.Infrastructures.Data
{
    public class TaskManagementDbContext : IdentityDbContext<ApplicationUsers,ApplicationRole,Guid>
    {
        public DbSet<ApplicationUsers> UserApplicationDb { get; set; }
        public DbSet<Tasks> TaskDb { get; set; }
        public DbSet<Category> CategoryDb { get; set; }

        public TaskManagementDbContext(DbContextOptions<TaskManagementDbContext> options): base(options) { }


        protected override void OnModelCreating(ModelBuilder builder)
        {
            
        }



    }

}
