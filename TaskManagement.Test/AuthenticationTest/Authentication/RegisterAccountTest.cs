using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Options;
using System.Net.Http.Json;
using TaskManagement.Test.HelperTest;
using TaskManagementApi.Application.Common.Settings;
using TaskManagementApi.Application.Features.Authentication.DTOs;
using TaskManagementApi.Domains.Wrapper;
using TaskManagementApi.PresentationUI;
using Xunit.Abstractions;


namespace TaskManagement.Test.AuthenticationTest.Authentication
{
    public class RegisterAccountTest : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;
        private readonly ITestOutputHelper _output;
        private readonly IOptions<IdentitySettings> _identitySettings;
        public RegisterAccountTest(CustomWebApplicationFactory<Program> factory,ITestOutputHelper output)
        {
            _output = output;
            Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Testing");
    
            _client = factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                BaseAddress = new Uri("https://localhost")
            });
        }

        [Fact]
        public async Task Controller_Should_Be_Discovered()
        {
            var response = await _client.GetAsync("/auth");
            // This should return 404 (no GET method), but not a connection error
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);
            var getResponse = response.StatusCode;
            _output.WriteLine($"Status Code: {getResponse.ToString()}");
        }
        [Fact]
        public async Task RegisterSuccess_ReturnsSuccessResponse()
        {
            var Assert = new AssertApiHelpers(_output);
        
             var registerData = new RegisterRequestDto(
         "useraaa123",  
            "admin3333@example.com",  
         "Test@1234"  
                 );
            
            // Log what we're sending
            var json = System.Text.Json.JsonSerializer.Serialize(registerData);
            _output.WriteLine($"Sending JSON: {json}");
        
            var registerResponse = await _client.PostAsJsonAsync("/auth/register", registerData);
            
            // Log the response details
            _output.WriteLine($"Response Status: {registerResponse.StatusCode}");

            var responseContent = await registerResponse.Content.ReadAsStringAsync();
            _output.WriteLine($"Response Content: {responseContent}");
             
        
        }
    }
}
