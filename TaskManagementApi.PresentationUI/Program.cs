using Serilog;
using Serilog.Events;
using TaskManagementApi.Application.Common.Settings;
using TaskManagementApi.PresentationUI.Extensions;

namespace TaskManagementApi.PresentationUI
{
    public class Program
    {
        /// <summary>
        /// Simplified Program.cs
        /// </summary>
        /// <param name="args"></param>
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            //Calling Service Extention
            // Add services to the container
            builder.AddCustomSerilog() // This comes first to capture startup logs
                   .Services
                   .AddPresentationService(builder.Configuration);

            var app = builder.Build();

            //Calling ApplicationBuilderExtensions 
            app?.ConfigureApplication();
           
            app?.Run();
        }
 
        
    }
}
