using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
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
            Console.WriteLine("Configuring WebHost for Testing...");
            builder.UseEnvironment("Testing"); // This allows your conditional UseInMemoryDb logic to run
            Console.WriteLine($"Environment set to: {builder.GetSetting(WebHostDefaults.EnvironmentKey)}");
            
            builder.ConfigureServices(services =>
            {
                Console.WriteLine("Configuring services for testing...");
                
                // Remove default DbContext
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<TaskManagementDbContext>));
                if (descriptor != null)
                {
                    services.Remove(descriptor);
                    Console.WriteLine("Removed existing DbContext configuration");
                }

                // Add in-memory database for testing
                services.AddDbContext<TaskManagementDbContext>(options =>
                {
                    options.UseInMemoryDatabase("InMemoryDbForTesting");
                });
                Console.WriteLine("Added in-memory database configuration");

                // Configure JWT settings for testing
                services.Configure<JwtSettings>(options =>
                {
                    options.Key = "TestJwtKeyForTestingPurposesOnlyThisShouldBeAtLeast256BitsLong";
                    options.Issuer = "TaskManagementApi";
                    options.Audience = "TaskManagementApiUsers";
                    options.ExpiryMinutes = 60;
                });
                services.AddSingleton(sp => sp.GetRequiredService<IOptions<JwtSettings>>().Value);
                Console.WriteLine("Configured JWT settings for testing");

                // Log all registered services to debug controller discovery
                Console.WriteLine("Registered services:");
                foreach (var service in services.Where(s => s.ServiceType.Name.Contains("Controller") || s.ServiceType.Name.Contains("Action")))
                {
                    Console.WriteLine($"  {service.ServiceType.Name} -> {service.ImplementationType?.Name ?? "Factory"}");
                }
            });

            builder.ConfigureAppConfiguration((context, config) =>
            {
                Console.WriteLine($"Configuring app configuration. Environment: {context.HostingEnvironment.EnvironmentName}");
                // Ensure the environment is set correctly
                config.AddInMemoryCollection(new Dictionary<string, string>
                {
                    ["ASPNETCORE_ENVIRONMENT"] = "Testing"
                });
            });
        }

        protected override IHost CreateHost(IHostBuilder builder)
        {
            Console.WriteLine("Creating host...");
            var host = base.CreateHost(builder);
            Console.WriteLine($"Host created. Environment: {host.Services.GetRequiredService<IHostEnvironment>().EnvironmentName}");
            
            // Seed the database after the host is created
            using var scope = host.Services.CreateScope();
            var scopedServices = scope.ServiceProvider;

            var db = scopedServices.GetRequiredService<TaskManagementDbContext>();
            var roleManager = scopedServices.GetRequiredService<RoleManager<ApplicationRole>>();
            var userManager = scopedServices.GetRequiredService<UserManager<ApplicationUsers>>();

            db.Database.EnsureCreated();
            Console.WriteLine("Database created and seeded");

            SeedTestData(userManager, roleManager).GetAwaiter().GetResult();
            Console.WriteLine("Test data seeded successfully");
            
            return host;
        }

        private async Task SeedTestData(UserManager<ApplicationUsers> userManager, RoleManager<ApplicationRole> roleManager)
        {
            // Ensure roles
            foreach (var role in new[] { "User", "Admin" })
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new ApplicationRole { Name = role });
                    Console.WriteLine($"Created role: {role}");
                }
            }

            // Add test user
            var email = "testuser@gmail.com";
            if (await userManager.FindByEmailAsync(email) is null)
            {
                var user = new ApplicationUsers
                {
                    UserName = email,
                    Email = email,
                    EmailConfirmed = true,
                    DomainUserId = Guid.NewGuid()
                };

                await userManager.CreateAsync(user, "Test@1234");
                await userManager.AddToRoleAsync(user, "User");
                Console.WriteLine($"Created test user: {email}");
            }
            else
            {
                Console.WriteLine($"Test user already exists: {email}");
            }
        }
    }
}
