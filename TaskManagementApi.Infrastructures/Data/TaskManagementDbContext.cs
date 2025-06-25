using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TaskManagement.Infrastructures.Data.Configuration;
using TaskManagement.Infrastructures.Data.Configurations;
using TaskManagement.Infrastructures.Identity.Models;
using TaskManagementApi.Domains.Entities;

namespace TaskManagement.Infrastructures.Data
{
    public class TaskManagementDbContext(DbContextOptions<TaskManagementDbContext> options)
        : IdentityDbContext<ApplicationUsers, ApplicationRole, Guid>(options)
    {
        public DbSet<ApplicationUsers> UserApplicationDb { get; set; }
        public DbSet<TaskItem> TaskDb { get; set; }
        public DbSet<Category> CategoryDb { get; set; }
        public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.ApplyConfiguration(new CategoriesConfiguration());
            builder.ApplyConfiguration(new RefreshTokenConfiguration());
            builder.ApplyConfiguration(new TasksConfiguration());
            builder.ApplyConfiguration(new UserConfiguration());
        }



    }

}
