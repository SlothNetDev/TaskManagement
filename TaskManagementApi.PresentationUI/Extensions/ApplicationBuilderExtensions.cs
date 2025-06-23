using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using TaskManagement.Infrastructures.Data.Seeders;
using TaskManagement.Infrastructures.Identity;

namespace TaskManagementApi.PresentationUI.Extensions
{
    public static class ApplicationBuilderExtensions
    {
        public static async Task<WebApplication> ConfigureApplication(this WebApplication app)
        {
             // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }

            //Seed Roles for request
            using(var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var roleManager = services.GetRequiredService<RoleManager<ApplicationRole>>();

                await IdentitySeeder.SeedRolesAsync(roleManager);
            }
            app.UseHttpsRedirection();


            app.UseAuthentication(); // Note: This should come BEFORE UseAuthorization
            app.UseAuthorization();
            

            app.MapControllers();

            return app;
        }
    }
}
