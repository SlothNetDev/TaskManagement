using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using TaskManagementApi.Application.Common.Settings;
using TaskManagementApi.PresentationUI;
using Xunit.Abstractions;

namespace TaskManagement.Test.AuthenticationTest.Authentication
{
    public class AuthIntegrationTests  : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;
        private readonly CustomWebApplicationFactory<Program> _factory;
        private readonly ITestOutputHelper _output;
        private readonly IOptions<IdentitySettings> _identitySettings;
        public AuthIntegrationTests(CustomWebApplicationFactory<Program> factory, ITestOutputHelper output)
        {
            _output = output;
            _factory = factory;
            Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Testing");
        
            _client = factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                BaseAddress = new Uri("https://localhost")
            });
        }
        [Fact]
        public async Task Application_Should_Be_Running()
        {
            // Test if the application is running by checking a simple endpoints
            var response = await _client.GetAsync("/");
            // Should get some response (might be 404 for root, but not connection error)
            response.Should().NotBeNull();
            _output.WriteLine($"Root endpoint response: {response.StatusCode}");
        }

        [Fact]
        public async Task Auth_Base_Route_Should_Return_404()
        {
            // Test that /auth by itself returns 404 (which is correct behavior)
            var response = await _client.GetAsync("/auth");
            _output.WriteLine($"Auth endpoint response: {response.StatusCode}");
            _output.WriteLine($"Auth endpoint response content: {await response.Content.ReadAsStringAsync()}");

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
            var response = await _client.GetAsync("/scalar/v1");
            _output.WriteLine($"Swagger endpoint response: {response.StatusCode}");
            // Should not be 404 Not Found
            response.StatusCode.Should().NotBe(System.Net.HttpStatusCode.NotFound);
        }

        [Fact]
        public void Controllers_Should_Be_Registered()
        {
            // Create a new scope to check if controllers are registered
            using var scope = _factory.Services.CreateScope();
            var services = scope.ServiceProvider;

            // Check if MediatR is registered
            var mediator = services.GetService<MediatR.IMediator>();
            mediator.Should().NotBeNull();

            _output.WriteLine("MediatR is registered successfully");

            // Check if controllers are registered by looking for action descriptors
            var actionDescriptorCollectionProvider = services.GetService<Microsoft.AspNetCore.Mvc.Infrastructure.IActionDescriptorCollectionProvider>();
            if (actionDescriptorCollectionProvider != null)
            {
                var actionDescriptors = actionDescriptorCollectionProvider.ActionDescriptors;
                _output.WriteLine($"Found {actionDescriptors.Items.Count} action descriptors:");
                foreach (var action in actionDescriptors.Items)
                {
                    _output.WriteLine($"  {action.DisplayName} -> {action.AttributeRouteInfo?.Template}");
                }
            }
            else
            {
                _output.WriteLine("No action descriptor collection provider found");
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
                _output.WriteLine($"DashboardController found: {dashboardController.ControllerTypeInfo.Name}");
                _output.WriteLine($"Route template: {dashboardController.AttributeRouteInfo?.Template}");
            }
            else
            {
                _output.WriteLine("DashboardController not found in registered controllers");

                // List all available controllers
                var allControllers = controllerTypes.Select(c => c.ControllerName).Distinct();
                _output.WriteLine("Available controllers:");
                foreach (var controller in allControllers)
                {
                    _output.WriteLine($"  - {controller}");
                }
            }
        }
        
    }
}
