using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.Extensions.Configuration;
using Npgsql.EntityFrameworkCore.PostgreSQL;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using Serilog.Events;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using TaskManagement.Infrastructures.Data;
using TaskManagement.Infrastructures.Identity.Models;
using TaskManagement.Infrastructures.Identity.Services;
using TaskManagement.Infrastructures.Services;
using TaskManagement.Infrastructures.Services.Categories;
using TaskManagement.Infrastructures.Services.Categories.Command;
using TaskManagement.Infrastructures.Services.Categories.Query;
using TaskManagement.Infrastructures.Services.TaskService;
using TaskManagement.Infrastructures.Services.TaskService.Command;
using TaskManagement.Infrastructures.Services.TaskService.Query;
using TaskManagementApi.Application.Common.Interfaces.IAuthentication;
using TaskManagementApi.Application.Common.Interfaces.ICategory.CategoryCommand;
using TaskManagementApi.Application.Common.Interfaces.ICategory.CategoryQuery;
using TaskManagementApi.Application.Common.Interfaces.ITask.TaskCommand;
using TaskManagementApi.Application.Common.Interfaces.ITaskItem.TaskCommand;
using TaskManagementApi.Application.Common.Interfaces.ITaskItem.TaskQuery;
using TaskManagementApi.Application.Common.Interfaces.IUser;
using TaskManagementApi.Application.Common.Interfaces.Repository;
using TaskManagementApi.Application.Common.Settings;
using TaskManagementApi.Application.Features.Authentication.Commands;
using TaskManagementApi.Application.Features.Authentication.DTOs;
using TaskManagementApi.Application.Features.CategoryFeature.Commands;
using TaskManagementApi.Application.Features.Task.Commands;
using TaskManagementApi.Core.IRepository.Categories;
using TaskManagementApi.Core.IRepository.Task;

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
            services.AddDbContext<ApplicationDbContext>((serviceProvider, options) =>
            {
                var env = serviceProvider.GetRequiredService<IHostEnvironment>();
                var config = serviceProvider.GetRequiredService<IConfiguration>();
                
                Console.WriteLine($"Environment in AddPresentationService: {env.EnvironmentName}");
                
                if (env.IsEnvironment("Testing"))
                {
                    Console.WriteLine("Using InMemoryDatabase for Testing");
                    options.UseInMemoryDatabase("InMemoryDbForTesting");
                }
                else
                {
                    Console.WriteLine("Using SQL Server for Production");
                    options.UseNpgsql(config.GetConnectionString("TaskDbConnection"));
                }
            });

            //Core service - Single AddControllers call with both assembly part and JSON configuration
            services.AddControllers()
                .AddApplicationPart(typeof(Controllers.DashboardController).Assembly)
                .ConfigureJsonOptions();  

            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            services.AddOpenApi(options =>
            {
                options.AddDocumentTransformer<BearerSecuritySchemeTransformer>();
            });

            //Dependency Injection 
            DependencyInjection(services, configuration);
            // Identity
            services.AddCustomIdentity();

            //add identity settings for roles
            services.Configure<IdentitySettings>(
                configuration.GetSection("IdentitySettings"));

            //Adding swaggers config
            Swaggers(services);

            //add authnetication
            AddAuthentication(services, configuration);
            services.AddAuthorization();
            services.AddHttpContextAccessor();
            //mediaR
            services.AddMediatR(x =>
            {
                x.RegisterServicesFromAssembly(typeof(RegisterCommand).Assembly);
                x.RegisterServicesFromAssembly(typeof(LoginCommand).Assembly);
                x.RegisterServicesFromAssembly(typeof(CreateCategoryCommand).Assembly);
                x.RegisterServicesFromAssembly(typeof(CreateTaskCommand).Assembly);
                x.RegisterServicesFromAssembly(typeof(UpdateCategoryCommand).Assembly);
                
            });
            //swagger end points
           /* services.AddEndpointsApiExplorer();*/
            /*services.AddSwaggerGen();*/

            return services;
        }
        #region Service Method
        private static void DependencyInjection(this IServiceCollection services, IConfiguration configuration)
        {
             // CQRS Services (keep only what you actually use)
            services.Configure<JwtSettings>(configuration.GetSection("JwtSettings"))
                .AddSingleton(sp => sp.GetRequiredService<IOptions<JwtSettings>>().Value);
            services.AddScoped<IAuthService, IdentityService>();
            services.AddScoped<IRefreshTokenService, RefreshTokenService>();
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IUpdateTaskService, UpdateTaskService>();
            services.AddScoped<IGetAllTask, GetAllTaskService>();
            services.AddScoped<ISearchTask, SearchTaskServices>();
            services.AddScoped<IGetDomainIdCategoryRepository, GetDomainIdCategoryRepository>();
            services.AddScoped<IDeleteTaskService, DeleteTaskService>();
            services.AddScoped<IPaganationTaskService, PaganationService>();
            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<ITaskRepository, TaskRepository>();
            services.AddScoped<IGetAllCategories, GetAllCategoriesService>();
            services.AddScoped<IGetDomainTaskRepository,GetDomainIdTaskRepository>();
            services.AddScoped<IDeleteCategoryService, DeleteCategoryService>();
        }

        private static void ConfigureJsonOptions(this IMvcBuilder builder)
        {
            builder.Services.Configure<JsonOptions>(options =>
            {
                //convert enum
                options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());

                //convert date time
                options.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
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
                .AddEntityFrameworkStores<ApplicationDbContext>();
        }

        private static void Swaggers(IServiceCollection services)
        {
            services.AddSwaggerGen(options =>
            {
                // Add JWT Authentication to Swagger
                options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = Microsoft.OpenApi.Models.ParameterLocation.Header,
                    Description = "Enter your JWT token below with `Bearer` prefix. Example: `Bearer eyJhbGci...`"
                });

                options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
                {
                    {
                        new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                        {
                            Reference = new Microsoft.OpenApi.Models.OpenApiReference
                            {
                                Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                });
            });

        }

        private static void AddAuthentication(IServiceCollection services,IConfiguration configuration)
        {
            // First: Configure JwtSettings properly
            services.Configure<JwtSettings>(configuration.GetSection("JwtSettings"));
            services.AddSingleton(sp => sp.GetRequiredService<IOptions<JwtSettings>>().Value); // Optional but safe

            //Then: configure JWT scheme
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
            {
                // Read settings from already-registered IOptions
                var serviceProvider = services.BuildServiceProvider(); // temporary provider
                var jwtSettings = serviceProvider.GetRequiredService<IOptions<JwtSettings>>().Value;

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = false,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings.Issuer,
                    ValidAudience = jwtSettings.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Key))
                };
            });

        }
#endregion
    }
    /// <summary>
    /// Serilog Class configuration
    /// </summary>
    public static class SerilogExtensions
    {
        public static WebApplicationBuilder AddCleanSerilog(this WebApplicationBuilder builder)
        {
            builder.Logging.ClearProviders();
            
            builder.Host.UseSerilog((context, services,loggerConfiguration) =>
            {
                loggerConfiguration.ReadFrom.Configuration(context.Configuration)
                .ReadFrom.Services(services);
            });
    
            return builder;
        }
    }
    internal sealed class BearerSecuritySchemeTransformer(Microsoft.AspNetCore.Authentication.IAuthenticationSchemeProvider authenticationSchemeProvider) : IOpenApiDocumentTransformer
    {
        public async Task TransformAsync(OpenApiDocument document, OpenApiDocumentTransformerContext context, CancellationToken cancellationToken)
        {
            var authenticationSchemes = await authenticationSchemeProvider.GetAllSchemesAsync();
            if (authenticationSchemes.Any(authScheme => authScheme.Name == "Bearer"))
            {
                var requirements = new Dictionary<string, OpenApiSecurityScheme>
                {
                    ["Bearer"] = new OpenApiSecurityScheme
                    {
                        Type = SecuritySchemeType.Http,
                        Scheme = "bearer", 
                        In = ParameterLocation.Header,
                        BearerFormat = "Json Web Token"
                    }
                };
                document.Components ??= new OpenApiComponents();
                document.Components.SecuritySchemes = requirements;
    
                foreach (var operation in document.Paths.Values.SelectMany(path => path.Operations))
                {
                    operation.Value.Security.Add(new OpenApiSecurityRequirement
                    {
                        [new OpenApiSecurityScheme { Reference = new OpenApiReference { Id = "Bearer", Type = ReferenceType.SecurityScheme } }] = Array.Empty<string>()
                    });
                }
            }
        }
    }

   
}
