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
    public class LoginAccountTest : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;
        private readonly ITestOutputHelper _output;
        public LoginAccountTest(CustomWebApplicationFactory<Program> factory, ITestOutputHelper output)
        {

            _output = output;
            Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Testing");
    
            _client = factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                BaseAddress = new Uri("https://localhost")
            });
        }
        private string userName = "useraaa123";
        private string email = "admin3333@example.com";
        private string password = "Test@1234";

        
        private async Task CreateAccount()
        {
             var Assert = new AssertApiHelpers(_output);
        
             var registerData = new RegisterRequestDto(
         userName,
         email,
         password
                 );
            
            // Log what we're sending
            var json = System.Text.Json.JsonSerializer.Serialize(registerData);
            _output.WriteLine($"Sending JSON: {json}");
        
            var registerResponse = await _client.PostAsJsonAsync("/auth/register", registerData);
            
            // Log the response details
            _output.WriteLine($"Response Status: {registerResponse.StatusCode}");

            var responseContent = registerResponse.Content.ReadAsStringAsync();
            _output.WriteLine($"Response Content: {responseContent}");
        
        }
        [Fact]
        public async Task LoginSuccess_ReturnsSuccessResponse()
        {
            var Assert = new AssertApiHelpers(_output);
            //call login Account
            await CreateAccount();

            // login
            var register = new LoginRequestDto(email, password);

            var json = System.Text.Json.JsonSerializer.Serialize(register);


            var registerResponse = await _client.PostAsJsonAsync("auth/login", register);
            // Log the response details
            _output.WriteLine($"Response Status: {registerResponse.StatusCode}");

            // Login
            var loginRequest = new LoginRequestDto(email, password);

            var responseContent = await registerResponse.Content.ReadAsStringAsync();
            _output.WriteLine($"Response Content: {responseContent}");

            var result = await registerResponse.Content.ReadFromJsonAsync<ResponseType<AuthResultDto>>();

        }

    }
}
