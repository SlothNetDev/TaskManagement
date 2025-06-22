
using Microsoft.AspNetCore.Http.Json;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using System.ComponentModel;
using System.Text.Json.Serialization;
using TaskManagement.Infrastructures.Data;
using TaskManagement.Infrastructures.Identity;
using TaskManagementApi.Application.Features.Authentication.Commands;
using TaskManagementApi.Application.Interfaces;
using TaskManagementApi.Application.Interfaces.IAuthentication;
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
            builder.Services.AddPresentationService(builder.Configuration);

            var app = builder.Build();

            //Calling ApplicationBuilderExtensions 
            app.ConfigureApplication();
           
            app.Run();
        }
    }
}
