
using FluentAssertions;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using TaskManagementApi.Application.Features.Authentication.DTOs;

namespace TaskManagement.Test.AuthenticationTest
{
    public class AuthTests(CustomWebApplicationFactory<TaskManagementApi.PresentationUI.Program> factory) : IClassFixture<CustomWebApplicationFactory<TaskManagementApi.PresentationUI.Program>>
    {
        private readonly HttpClient _client = factory.CreateClient(); 

        [Fact]
        public async Task Register_And_Login_Should_Return_JwtToken()
        {
            //1. register
            var registerRequest = new RegisterRequestDto(
                "Babakatbonaks",
                "testuser@gmail.com",
                "paSSword@#SS551");

            var registerResponse = await _client.PostAsJsonAsync("/regsiter", registerRequest);
            registerResponse.EnsureSuccessStatusCode();

            //2.  Login Account
            var loginRequest = new LoginRequestDto(
                "testuser@gmail.com",
                "paSSword@#SS551");


            var loginResponse = await _client.PostAsJsonAsync("/login", loginRequest);
            loginResponse.EnsureSuccessStatusCode();

            var result = await loginResponse.Content.ReadFromJsonAsync<LoginResponse>();
            result.Should().NotBeNull();
            result!.Token.Should().NotBeNullOrEmpty();

        }
        [Fact]
        public async Task GetProfile_With_ValidToken_Should_Return_UserProfile()
        {
            var token = await GetJwtToken(); // Helper to login and get token

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _client.GetAsync("account/profile");
            response.EnsureSuccessStatusCode();
        }

        public class LoginResponse
        {
            public bool Success { get; set; }
            public string Token { get; set; } = string.Empty;
            public string RefreshToken { get; set; } = string.Empty;
            public string Message { get; set; } = string.Empty;
        }
        private async Task<string> GetJwtToken()
        {
            var loginRequest = new LoginRequestDto(
                "testuser@gmail.com",
                "paSSword@#SS551");
            var response = await _client.PostAsJsonAsync("/login", loginRequest);
            var result = await response.Content.ReadFromJsonAsync<LoginResponse>();
            return result!.Token;
        }


    }
}
