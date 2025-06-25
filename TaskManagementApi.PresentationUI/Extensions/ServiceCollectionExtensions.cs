using Microsoft.AspNetCore.Http.Json;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Serilog;
using Serilog.Events;
using System.Text.Json.Serialization;
using TaskManagement.Infrastructures.Data;
using TaskManagement.Infrastructures.Identity.Models;
using TaskManagement.Infrastructures.Identity.Services;
using TaskManagementApi.Application.Common.Interfaces.IAuthentication;
using TaskManagementApi.Application.Common.Settings;
using TaskManagementApi.Application.Features.Authentication.Commands;
using TaskManagementApi.Application.Features.Authentication.DTOs;

namespace TaskManagementApi.PresentationUI.Extensions
{
    /// <summary>
    /// Uses for builder Service
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddPresentationService(this IServiceCollection services,
            IConfiguration configuration )
        {
            //Core service
            services.AddControllers()
                .ConfigureJsonOptions();

            //database
            services.AddDbContext<TaskManagementDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("TaskDbConnection")));

            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            services.AddOpenApi();

             // CQRS Services (keep only what you actually use)
            services.Configure<JwtSettings>(configuration.GetSection("JwtSettings"))
                .AddSingleton(sp => sp.GetRequiredService<IOptions<JwtSettings>>().Value);
            services.AddScoped<IAuthService, IdentityService>();
            services.AddScoped<IRefreshTokenService, RefreshTokenService>();
            services.AddScoped<ITokenService, TokenService>();

            // Identity
            services.AddCustomIdentity();

            //add identity settings for roles
            services.Configure<IdentitySettings>(
                configuration.GetSection("IdentitySettings"));

            //Authentication/Autherization
            services.AddAuthentication();
            services.AddAuthorization();

            //mediaR
            services.AddMediatR(x =>
            {
                x.RegisterServicesFromAssembly(typeof(RegisterCommand).Assembly);
                x.RegisterServicesFromAssembly(typeof(LoginCommand).Assembly);
            });

            //swagger end points
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();

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
    public static class SerilogExtensions
    {
        public static WebApplicationBuilder AddCleanSerilog(this WebApplicationBuilder builder)
        {
            builder.Logging.ClearProviders();
            
            builder.Host.UseSerilog((context, config) =>
            {
                config.ReadFrom.Configuration(context.Configuration)
                      .Enrich.FromLogContext();
            });
    
            return builder;
        }
    }
}
