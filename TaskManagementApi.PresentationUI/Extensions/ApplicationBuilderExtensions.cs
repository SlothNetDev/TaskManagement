using Microsoft.AspNetCore.Identity;
using Scalar.AspNetCore;
using System.Threading.Tasks;
using TaskManagement.Infrastructures.Data.Seeders;
using TaskManagement.Infrastructures.Identity.Models;
using TaskManagementApi.PresentationUI.Middleware;

namespace TaskManagementApi.PresentationUI.Extensions
{
    public static class ApplicationBuilderExtensions
    {
        public static async Task<WebApplication> ConfigureApplication(this WebApplication app)
        {
            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment() || app.Environment.IsEnvironment("Testing"))
            {
                app.UseDeveloperExceptionPage();
                app.MapOpenApi();
                app.MapScalarApiReference(options =>
                {
                    options.Title = "Task Management System";
                    options.Theme = ScalarTheme.BluePlanet;
                    options.HideClientButton = true;
                    options.DefaultHttpClient = new(ScalarTarget.CSharp, ScalarClient.HttpClient);
                });
            }

            //Seed Roles for request (skip in Testing environment - handled by test factory)
            if (!app.Environment.IsEnvironment("Testing"))
            {
                using(var scope = app.Services.CreateScope())
                {
                    var services = scope.ServiceProvider;
                    var roleManager = services.GetRequiredService<RoleManager<ApplicationRole>>();

                    await IdentitySeeder.SeedRolesAsync(roleManager);
                }
            }
            
            //adding global middleware
            app.UseMiddleware<MiddlewareException>();

            app.UseHttpsRedirection();

            app.UseAuthentication(); // Note: This should come BEFORE UseAuthorization
            app.UseAuthorization();
            
            app.MapControllers();

            return app;
        }
    }
}
