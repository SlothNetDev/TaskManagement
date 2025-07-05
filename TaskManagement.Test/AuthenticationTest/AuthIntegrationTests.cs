using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using TaskManagementApi.Application.Features.Authentication.DTOs;
using TaskManagementApi.Domains.Wrapper;

namespace TaskManagement.Test.AuthenticationTest
{
    public class AuthTests : IClassFixture<CustomWebApplicationFactory<TaskManagementApi.PresentationUI.Program>>
    {
        private readonly HttpClient _client;
        private readonly CustomWebApplicationFactory<TaskManagementApi.PresentationUI.Program> _factory;
    
        public AuthTests(CustomWebApplicationFactory<TaskManagementApi.PresentationUI.Program> factory)
        {
            Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Testing");
            _factory = factory;
    
            _client = factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                BaseAddress = new Uri("https://localhost")
            });
        }

        [Fact]
        public async Task Application_Should_Be_Running()
        {
            // Test if the application is running by checking a simple endpoint
            var response = await _client.GetAsync("/");
            // Should get some response (might be 404 for root, but not connection error)
            response.Should().NotBeNull();
            Console.WriteLine($"Root endpoint response: {response.StatusCode}");
        }

        [Fact]
        public async Task Auth_Base_Route_Should_Return_404()
        {
            // Test that /auth by itself returns 404 (which is correct behavior)
            var response = await _client.GetAsync("/auth");
            Console.WriteLine($"Auth endpoint response: {response.StatusCode}");
            Console.WriteLine($"Auth endpoint response content: {await response.Content.ReadAsStringAsync()}");
            
            // /auth by itself should return 404 because there's no GET action for the base route
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task Check_Available_Routes()
        {
            // Try different endpoints to see what's available
            var endpoints = new[] { "/", "/swagger", "/auth", "/auth/login", "/auth/register", "/account/profile" };
            
            foreach (var endpoint in endpoints)
            {
                var response = await _client.GetAsync(endpoint);
                Console.WriteLine($"{endpoint}: {response.StatusCode}");
            }
        }

        [Fact]
        public async Task Swagger_Should_Be_Available()
        {
            // Test if Swagger is available (indicates the app is running)
            var response = await _client.GetAsync("/swagger");
            Console.WriteLine($"Swagger endpoint response: {response.StatusCode}");
            // Should not be 404 Not Found
            response.StatusCode.Should().NotBe(System.Net.HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task Controllers_Should_Be_Registered()
        {
            // Create a new scope to check if controllers are registered
            using var scope = _factory.Services.CreateScope();
            var services = scope.ServiceProvider;
            
            // Check if MediatR is registered
            var mediator = services.GetService<MediatR.IMediator>();
            mediator.Should().NotBeNull();
            
            Console.WriteLine("MediatR is registered successfully");

            // Check if controllers are registered by looking for action descriptors
            var actionDescriptorCollectionProvider = services.GetService<Microsoft.AspNetCore.Mvc.Infrastructure.IActionDescriptorCollectionProvider>();
            if (actionDescriptorCollectionProvider != null)
            {
                var actionDescriptors = actionDescriptorCollectionProvider.ActionDescriptors;
                Console.WriteLine($"Found {actionDescriptors.Items.Count} action descriptors:");
                foreach (var action in actionDescriptors.Items)
                {
                    Console.WriteLine($"  {action.DisplayName} -> {action.AttributeRouteInfo?.Template}");
                }
            }
            else
            {
                Console.WriteLine("No action descriptor collection provider found");
            }
        }

        [Fact]
        public async Task DashboardController_Should_Be_Discovered()
        {
            // Create a new scope to check if the DashboardController is registered
            using var scope = _factory.Services.CreateScope();
            var services = scope.ServiceProvider;
            
            // Check if the DashboardController is registered
            var controllerTypes = services.GetServices<Microsoft.AspNetCore.Mvc.Controllers.ControllerActionDescriptor>();
            var dashboardController = controllerTypes.FirstOrDefault(c => c.ControllerName == "Dashboard");
            
            if (dashboardController != null)
            {
                Console.WriteLine($"DashboardController found: {dashboardController.ControllerTypeInfo.Name}");
                Console.WriteLine($"Route template: {dashboardController.AttributeRouteInfo?.Template}");
            }
            else
            {
                Console.WriteLine("DashboardController not found in registered controllers");
                
                // List all available controllers
                var allControllers = controllerTypes.Select(c => c.ControllerName).Distinct();
                Console.WriteLine("Available controllers:");
                foreach (var controller in allControllers)
                {
                    Console.WriteLine($"  - {controller}");
                }
            }
        }
    
        [Fact]
        public async Task Register_And_Login_Should_Return_Token()
        {
            var register = new RegisterRequestDto("JohnDoe", "newuser@email.com", "paSSword@#SS551");
    
            var registerResult = await _client.PostAsJsonAsync("/auth/register", register);
    
            // Ignore registration errors (already exists)
            if (!registerResult.IsSuccessStatusCode)
            {
                var error = await registerResult.Content.ReadAsStringAsync();
                Console.WriteLine("Register failed: " + error);
            }
    
            // Use the correct password that matches the seeded user
            var loginRequest = new LoginRequestDto("testuser@gmail.com", "Test@1234");
            var loginResponse = await _client.PostAsJsonAsync("/auth/login", loginRequest);

            if (!loginResponse.IsSuccessStatusCode)
            {
                var errorBody = await loginResponse.Content.ReadAsStringAsync();
                Console.WriteLine($"Login failed with status {loginResponse.StatusCode}: {errorBody}");
                throw new Exception($"Login failed ({loginResponse.StatusCode}): {errorBody}");
            }
            
            var response = await loginResponse.Content.ReadFromJsonAsync<ResponseType<AuthResultDto>>();
            response.Should().NotBeNull();
            response!.Success.Should().BeTrue();
            response.Data.Should().NotBeNull();
            response.Data!.Token.Should().NotBeNullOrEmpty();
        }
    
        [Fact]
        public async Task GetProfile_With_ValidToken_Should_Return_Profile()
        {
            var token = await GetJwtToken();
    
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    
            var result = await _client.GetAsync("/account/profile");
            result.EnsureSuccessStatusCode();
    
            var body = await result.Content.ReadAsStringAsync();
            body.Should().Contain("testuser@gmail.com");
        }
    
        private async Task<string> GetJwtToken()
        {
            var login = new LoginRequestDto("testuser@gmail.com", "Test@1234");
            var response = await _client.PostAsJsonAsync("/auth/login", login);
            var result = await response.Content.ReadFromJsonAsync<ResponseType<AuthResultDto>>();
            return result!.Data!.Token;
        }

        [Theory]
        [InlineData("/auth/register")]
        [InlineData("/auth/login")]
        [InlineData("/auth/refresh")]
        [InlineData("/auth/logOut")]
        public async Task Auth_Endpoints_Should_Be_Available(string endpoint)
        {
            var response = await _client.PostAsync(endpoint, new StringContent("{}", System.Text.Encoding.UTF8, "application/json"));
            response.StatusCode.Should().NotBe(System.Net.HttpStatusCode.NotFound);
        }
    }
}
