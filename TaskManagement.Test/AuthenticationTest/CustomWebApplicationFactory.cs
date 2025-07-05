using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.InMemory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.SqlServer;
using TaskManagement.Infrastructures.Data;
using TaskManagement.Infrastructures.Identity.Models;
using TaskManagementApi.Application.Features.Authentication.DTOs;
namespace TaskManagement.Test.AuthenticationTest
{
    public class CustomWebApplicationFactory<TProgram> : WebApplicationFactory<TProgram> where TProgram : class
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                //remove all dbcontext
                services.RemoveAll<TaskManagementDbContext>();
                // Optional: Replace real DB with in-memory for tests



                 // Let's try removing existing Identity registrations first if they exist and causing issues.
                // This is more aggressive but ensures a clean slate for Identity setup.
                var identityDescriptor = services.FirstOrDefault(d => d.ServiceType == typeof(UserManager<ApplicationUser>));
                if (identityDescriptor != null)
                {
                    // Remove all services related to Identity and its stores
                    // This is a bit of a blunt instrument, but often necessary for clean test setup.
                    services.RemoveAll(typeof(IUserStore<>));
                    services.RemoveAll(typeof(IRoleStore<>));
                    services.RemoveAll(typeof(UserManager<>));
                    services.RemoveAll(typeof(SignInManager<>));
                    services.RemoveAll(typeof(RoleManager<>));
                    // Add more if your Identity setup added other specific services.
                }
                // Now, add ApplicationDbContext using an in-memory database for testing
                services.AddDbContext<TaskManagementDbContext>(options =>
                {
                    options.UseInMemoryDatabase("TaskDbForTesting"); // Ensure a unique name if you have multiple factories
                });
                 // Re-add Identity services for testing, pointing to the in-memory DbContext
                services.AddIdentity<TaskManagementDbContext, IdentityRole>(options =>
                {
                    // Relax password requirements for testing (adjust as needed)
                    options.Password.RequireDigit = false;
                    options.Password.RequiredLength = 4;
                    options.Password.RequireNonAlphanumeric = false;
                    options.Password.RequireUppercase = false;
                    options.Password.RequireLowercase = false;
                    options.Password.RequiredUniqueChars = 0;

                    // Disable lockout for testing
                    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(0);
                    options.Lockout.MaxFailedAccessAttempts = 999;
                    options.Lockout.AllowedForNewUsers = false;

                    // Configure user options
                    options.User.RequireUniqueEmail = true;
                })
                .AddEntityFrameworkStores<TaskManagementDbContext>() // Point to the in-memory context
                .AddDefaultTokenProviders();

                // Build the service provider
                var sp = services.BuildServiceProvider();

                // Create a scope to obtain a reference to the database contexts
                using (var scope = sp.CreateScope())
                {
                    var scopedServices = scope.ServiceProvider;
                    var dbContext = scopedServices.GetRequiredService<TaskManagementDbContext>();
                    var userManager = scopedServices.GetRequiredService<UserManager<TaskManagementDbContext>>();
                    var roleManager = scopedServices.GetRequiredService<RoleManager<IdentityRole>>();

                    // Ensure the database is created.
                    // For in-memory, this creates the schema.
                    dbContext.Database.EnsureCreated();

                    // Seed the database with test data if necessary
                    SeedData(dbContext, userManager, roleManager);
                }
        });
    }

            // Helper method to seed data for tests
            private void SeedData(TaskManagementDbContext context, UserManager<TaskManagementDbContext> userManager, RoleManager<IdentityRole> roleManager)
            {
                // Ensure any roles needed for tests exist
                if (!roleManager.RoleExistsAsync("Admin").Result)
                {
                    roleManager.CreateAsync(new IdentityRole("Admin")).Wait();
                }
                if (!roleManager.RoleExistsAsync("User").Result) // Example: add a default User role
                {
                    roleManager.CreateAsync(new IdentityRole("User")).Wait();
                }

               
            }
     
    }
}
