
using Microsoft.AspNetCore.Http.Json;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.Text.Json.Serialization;
using TaskManagement.Infrastructures.Data;
using TaskManagement.Infrastructures.Identity;

namespace TaskManagementApi.PresentationUI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();

            //Adding enums and Datetime conversion
            builder.Services.Configure<JsonOptions>(options =>
            {
                //convert enum
                options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());

                //convert date time
                options.SerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
                options.SerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;

            });
            //Adding Database sql
            builder.Services.AddDbContext<TaskManagementDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("TaskDbConnection")));

            //If you're doing [Authorize] and roles:
            builder.Services.AddAuthentication();
            builder.Services.AddAuthorization();


            //add identity
            builder.Services.AddIdentity<ApplicationUsers, ApplicationRole>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequiredLength = 8;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequireUppercase = true;
            })
                .AddDefaultTokenProviders()
                .AddEntityFrameworkStores<TaskManagementDbContext>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();
            app.UseAuthentication();

            app.MapControllers();

            app.Run();
        }
    }
}
