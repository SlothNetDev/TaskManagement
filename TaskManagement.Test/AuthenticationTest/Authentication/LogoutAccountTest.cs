using Microsoft.AspNetCore.Mvc.Testing;
using System.Net.Http.Json;
using TaskManagementApi.WebAPI;
using Xunit.Abstractions;
using TaskManagementApi.Application.Features.Authentication.DTOs;
using TaskManagement.Test.HelperTest;
using TaskManagementApi.Domains.Wrapper;

namespace TaskManagement.Test.AuthenticationTest.Authentication
{
    public class LogoutAccountTest : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;
        private readonly ITestOutputHelper _output;
        public LogoutAccountTest(CustomWebApplicationFactory<Program> factory,ITestOutputHelper output)
        {
            _output = output;
            Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Testing");
    
            _client = factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                BaseAddress = new Uri("https://localhost")
            });
        }
        private string UniqueEmail() => $"testuser_{Guid.NewGuid()}@example.com";
        private string UniqueUsername() => $"testuser_{Guid.NewGuid()}";
        private const string Password = "Test@1234";

        
    }
}
