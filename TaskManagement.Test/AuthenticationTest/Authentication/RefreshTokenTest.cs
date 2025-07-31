using Microsoft.AspNetCore.Mvc.Testing;
using System.Net.Http.Json;
using TaskManagementApi.WebAPI;
using Xunit.Abstractions;
using TaskManagementApi.Application.Features.Authentication.DTOs;
using TaskManagement.Test.HelperTest;
using TaskManagementApi.Domains.Wrapper;
using TaskManagementApi.Application.Features.CategoryFeature.CategoriesDto;
using System.Text.Json;

namespace TaskManagement.Test.AuthenticationTest.Authentication
{
    public class RefreshTokenTest : IClassFixture<CustomWebApplicationFactory<TaskManagementApi.WebAPI.Program>>
    {
        private readonly HttpClient _client;
        private readonly ITestOutputHelper _output;
        public RefreshTokenTest(CustomWebApplicationFactory<Program> factory,ITestOutputHelper output)
        {
            _output = output;
            Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Testing");
    
            _client = factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                BaseAddress = new Uri("https://localhost")
            });
        }


        private async Task CreateAccount()
        {
            //create
            string _testUsername = "testuser";
            string _testEmail =  "testuser@example.com";
            const string Password = "Test@1234"; 
            
            var createUser = new RegisterRequestDto(
             _testUsername,
                _testEmail,
                Password);

            //serilize the create User
            var json = System.Text.Json.JsonSerializer.Serialize(createUser);
            _output.WriteLine($"Sending Json: {json}");

            //register to auth/register
            var register = await _client.PostAsJsonAsync("/auth/register",createUser);

            //Log the response Details for status code
            _output.WriteLine($"Response Status: {register.StatusCode}");

            //get the response for content
            var responseContent = await register.Content.ReadAsStringAsync();
            _output.WriteLine($"Response Content: {responseContent}");

        }
        private async Task<ResponseType<AuthResultDto>> LoginAccount()
        {
            await CreateAccount();
            //create
            string _testUsername = "testuser";
            string _testEmail =  "testuser@example.com";
            const string Password = "Test@1234"; 
            var loginRequest = new LoginRequestDto(_testEmail, Password);
            var loginResponse = await _client.PostAsJsonAsync("auth/login", loginRequest);
        
            // Ensure the response is successful
            if (!loginResponse.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"Login failed with status: {loginResponse.StatusCode}");
            }
        
            // Read and log the raw response
            var responseContent = await loginResponse.Content.ReadAsStringAsync();
            _output.WriteLine($"Raw login response: {responseContent}");
        
            // Deserialize with proper options
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
        
            var result = await loginResponse.Content.ReadFromJsonAsync<ResponseType<AuthResultDto>>(options);
            _output.WriteLine($"Token expires at: {result.Data.ExpiresAt}");
            if (result == null)
            {
                throw new InvalidOperationException("Failed to deserialize login response");
            }
        
            _output.WriteLine($"Deserialized result - Success: {result.Success}, Message: {result.Message}");
        
            if (!result.Success)
            {
                throw new InvalidOperationException(result.Message ?? "Login failed");
            }
        
            if (result.Data == null)
            {
                throw new InvalidOperationException("Login succeeded but no data was returned");
            }
        
            return result;
        }

        [Fact]
        public async Task RefreshToken_ReturnsNewAccessToken()
        {
            // Login first
            var loginResult = await LoginAccount();
        
            // Prepare refresh request
            var refreshRequest = new 
            { 
                Token = loginResult.Data.BearerToken,
                RefreshToken = loginResult.Data.RefreshToken 
            };
        
            // Log the tokens being sent
            _output.WriteLine($"Bearer Token: {loginResult.Data.BearerToken}");
            _output.WriteLine($"Refresh Token: {loginResult.Data.RefreshToken}");
        
            // Send refresh request
            var refreshResponse = await _client.PostAsJsonAsync("/auth/refresh", refreshRequest);
        
            // Check if the request was successful
            if (!refreshResponse.IsSuccessStatusCode)
            {
                var errorContent = await refreshResponse.Content.ReadAsStringAsync();
                _output.WriteLine($"Refresh failed. Status: {refreshResponse.StatusCode}, Error: {errorContent}");
                throw new HttpRequestException($"Refresh failed: {errorContent}");
            }
        
            // Deserialize the successful response
            var refreshResult = await refreshResponse.Content.ReadFromJsonAsync<ResponseType<AuthResultDto>>(
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        
            if (refreshResult == null || !refreshResult.Success || refreshResult.Data == null)
            {
                throw new InvalidOperationException("Refresh token request failed or returned invalid data");
            }

            var Assert = new AssertApiHelpers(_output);

            Assert.ShouldSucceed(refreshResult, "Successfully refreshed the token.");
        }
    }
}
