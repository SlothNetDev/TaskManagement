using Microsoft.AspNetCore.Http.Json;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using TaskManagement.Infrastructures.Data;
using TaskManagement.Infrastructures.Identity;
using TaskManagementApi.Application.Common.Interfaces.IAuthentication;
using TaskManagementApi.Application.Common.Settings;
using TaskManagementApi.Application.Features.Authentication.Commands;
using TaskManagementApi.Application.Interfaces;

namespace TaskManagementApi.PresentationUI.Extensions
{
    /// <summary>
    /// Uses for builder Service
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddPresentationService(this IServiceCollection services,
            IConfiguration configuration)
        {
            //Core service
            services.AddControllers()
                .ConfigureJsonOptions();

            //database
            services.AddDbContext<TaskManagementDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("TaskDbConnection")));

            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            services.AddOpenApi();

            // Identity
            services.AddCustomIdentity();

            //add identity settings for roles
            services.Configure<IdentitySettings>(
                configuration.GetSection("IdentitySettings"));

            //Authentication/Autherization
            services.AddAuthentication();
            services.AddAuthorization();

            // CQRS Services (keep only what you actually use)
            services.AddScoped<IRegisterCommand, RegisterCommand>();
            /*services.AddScoped<ITokenService, TokenService>();*/
            // services.AddScoped<ILoginCommand, LoginCommand>();

            return services;
        }

        private static void ConfigureJsonOptions(this IMvcBuilder builder)
        {
            builder.Services.Configure<JsonOptions>(options =>
            {
                //convert enum
                options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());

                //convert date time
                options.SerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
                options.SerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;

            });
        }

        private static void AddCustomIdentity(this IServiceCollection services)
        {
            //add identity
            services.AddIdentity<ApplicationUsers, ApplicationRole>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequiredLength = 8;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequireUppercase = true;
            })
                .AddDefaultTokenProviders()
                .AddEntityFrameworkStores<TaskManagementDbContext>();
        }
    }
}
