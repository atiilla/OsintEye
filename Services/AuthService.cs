using System.Net.Http.Json;
using System.Text.Json;
using MauiApp1.Models;

namespace MauiApp1.Services
{
    public class AuthService
    {
        private readonly HttpClient _httpClient;
        private const string BaseUrl = "http://192.168.0.160:5062/api";

        public AuthService()
        {
            try
            {
                var handler = new HttpClientHandler();
                handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true;
                _httpClient = new HttpClient(handler);
                _httpClient.Timeout = TimeSpan.FromSeconds(30); // Set timeout to 30 seconds
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"HttpClient initialization error: {ex.Message}");
                throw;
            }
        }

        public async Task<bool> Login(string email, string password)
        {
            try
            {
                var loginRequest = new LoginRequest
                {
                    Email = email,
                    Password = password,
                    RememberMe = true
                };

                System.Diagnostics.Debug.WriteLine($"Attempting to connect to: {BaseUrl}/User/login");
                System.Diagnostics.Debug.WriteLine($"Request payload: {JsonSerializer.Serialize(loginRequest)}");

                using var request = new HttpRequestMessage(HttpMethod.Post, $"{BaseUrl}/User/login");
                request.Content = JsonContent.Create(loginRequest);
                
                // Add headers for debugging
                request.Headers.Add("Accept", "application/json");
                
                var response = await _httpClient.SendAsync(request);
                var content = await response.Content.ReadAsStringAsync();

                System.Diagnostics.Debug.WriteLine($"Response status code: {response.StatusCode}");
                System.Diagnostics.Debug.WriteLine($"Response content: {content}");

                if (response.IsSuccessStatusCode)
                {
                    return true;
                }
                
                System.Diagnostics.Debug.WriteLine($"Request failed with status: {response.StatusCode}");
                return false;
            }
            catch (HttpRequestException ex)
            {
                System.Diagnostics.Debug.WriteLine($"HTTP Request error: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Inner exception: {ex.InnerException?.Message}");
                return false;
            }
            catch (TaskCanceledException ex)
            {
                System.Diagnostics.Debug.WriteLine($"Request timed out: {ex.Message}");
                return false;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Unexpected error: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");
                return false;
            }
        }
    }
} 