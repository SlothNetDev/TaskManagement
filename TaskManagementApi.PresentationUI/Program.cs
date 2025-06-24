using Serilog;
using Serilog.Events;
using System.Threading.Tasks;
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
        public static async Task Main()
        {
            var builder = WebApplication.CreateBuilder();

            //Calling Service Extention
            // Add services to the container
            builder.AddCleanSerilog() // This comes first to capture startup logs
                   .Services
                   .AddPresentationService(builder.Configuration);

            var app = builder.Build();

            //Calling ApplicationBuilderExtensions 
            await app.ConfigureApplication();
           
            app?.Run();
        }
 
        
    }
}
