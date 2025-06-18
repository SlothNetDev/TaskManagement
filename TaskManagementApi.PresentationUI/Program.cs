
using Microsoft.AspNetCore.Http.Json;
using System.ComponentModel;
using System.Text.Json.Serialization;

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
            
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
