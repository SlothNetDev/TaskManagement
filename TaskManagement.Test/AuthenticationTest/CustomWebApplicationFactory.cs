using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TaskManagement.Infrastructures.Data;
using TaskManagement.Infrastructures.Identity.Models;
using TaskManagementApi.Application.Common.Settings;
using Microsoft.Extensions.Options;

namespace TaskManagement.Test.AuthenticationTest
{
    public class CustomWebApplicationFactory<TProgram> : WebApplicationFactory<TProgram> where TProgram : class
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("Testing");

            builder.ConfigureServices(services =>
            {
                // Find and remove ALL DbContext-related services to ensure a clean slate
                // This is more robust than just removing the DbContextOptions descriptor
                var dbContextRelatedServices = services
                    .Where(s => s.ServiceType.IsGenericType &&
                                 s.ServiceType.GetGenericTypeDefinition() == typeof(DbContextOptions<>) ||
                                 s.ServiceType == typeof(ApplicationDbContext) ||
                                 (s.ServiceType.Name.Contains("DbContext") && s.ServiceType.FullName.Contains("Microsoft.EntityFrameworkCore")))
                    .ToList(); // Materialize to avoid modifying collection while iterating

                foreach (var service in dbContextRelatedServices)
                {
                    services.Remove(service);
                }

                // A more targeted removal for DbContextOptions<ApplicationDbContext>
                var descriptorToRemove = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));
                if (descriptorToRemove != null)
                {
                    services.Remove(descriptorToRemove);
                }

                // ALSO remove the ApplicationDbContext service itself, if directly registered
                var applicationDbContextService = services.SingleOrDefault(
                    d => d.ServiceType == typeof(ApplicationDbContext) && d.Lifetime == ServiceLifetime.Scoped);
                if (applicationDbContextService != null)
                {
                    services.Remove(applicationDbContextService);
                }
                
                // Add in-memory database
                // Use `ServiceLifetime.Scoped` if your DbContext is scoped (which is the default and recommended).
                services.AddDbContext<ApplicationDbContext>(options =>
                {
                    options.UseInMemoryDatabase("InMemoryDbForTesting");
                }, ServiceLifetime.Scoped); // Explicitly set lifetime if not already default

                 // Configure IdentitySettings for testing
                services.Configure<IdentitySettings>(options =>
                {
                    options.AdminEmails = new List<string>
                    {
                        "admin@example.com", "superadmin@example.com"
                    };
                });

                // Configure JWT settings for testings
                services.Configure<JwtSettings>(options =>
                {
                    options.Key = "TestJwtKeyForTestingPurposesOnlyThisShouldBeAtLeast256BitsLong";
                    options.Issuer = "TaskManagementApi";
                    options.Audience = "TaskManagementApiUsers";
                    options.ExpiryMinutes = 60;
                });
                services.AddSingleton(sp => sp.GetRequiredService<IOptions<JwtSettings>>().Value);
            });

            builder.ConfigureAppConfiguration((context, config) =>
            {
                config.AddInMemoryCollection(new Dictionary<string, string>
                {
                    ["ASPNETCORE_ENVIRONMENT"] = "Testing"
                });
            });
        }
        
        protected override IHost CreateHost(IHostBuilder builder)
        {
            var host = base.CreateHost(builder);

            // Seed the database after the host is created
            using var scope = host.Services.CreateScope();
            var scopedServices = scope.ServiceProvider;

            var db = scopedServices.GetRequiredService<ApplicationDbContext>();
            var roleManager = scopedServices.GetRequiredService<RoleManager<ApplicationRole>>();
            var userManager = scopedServices.GetRequiredService<UserManager<ApplicationUsers>>();

            // EnsureCreated is crucial for InMemoryDb as it doesn't have migrations
            db.Database.EnsureCreated();
            
            SeedTestData(userManager, roleManager).GetAwaiter().GetResult();

            return host;
        }
        
        private async Task SeedTestData(UserManager<ApplicationUsers> userManager, RoleManager<ApplicationRole> roleManager)
        {
            // Ensure roles
            foreach (var role in new[] { "Admin","User" })
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new ApplicationRole { Name = role });
                }
            }
        }
    }
}