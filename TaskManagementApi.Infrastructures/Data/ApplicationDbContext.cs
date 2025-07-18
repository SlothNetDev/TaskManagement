using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TaskManagement.Infrastructures.Data.Configurations;
using TaskManagement.Infrastructures.Data.Configurations;
using TaskManagement.Infrastructures.Identity.Models;
using TaskManagementApi.Domains.Entities;

namespace TaskManagement.Infrastructures.Data
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : IdentityDbContext<ApplicationUsers, ApplicationRole, Guid>(options)
    {
        //set as virtual to make them accessible for mock
        public virtual DbSet<ApplicationUsers> UserApplicationDb { get; set; }
        public virtual DbSet<TaskItem> TaskDb { get; set; }
        public virtual DbSet<Category> CategoryDb { get; set; }
        public virtual DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

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
