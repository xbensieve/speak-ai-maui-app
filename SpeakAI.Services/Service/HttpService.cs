using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SpeakAI.Services.Service
{
    public class HttpService
    {
        private readonly HttpClient _httpClient;

        public HttpService(HttpClient httpClient)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        }

        // Generic POST request
        public async Task<TResponse> PostAsync<TRequest, TResponse>(string url, TRequest requestData)
        {
            return await SendRequestAsync<TRequest, TResponse>(HttpMethod.Post, url, requestData);
        }

        // Generic PUT request
        public async Task<TResponse> PutAsync<TRequest, TResponse>(string url, TRequest requestData)
        {
            return await SendRequestAsync<TRequest, TResponse>(HttpMethod.Put, url, requestData);
        }

        // Generic GET request
        public async Task<TResponse> GetAsync<TResponse>(string url)
        {
            return await SendRequestAsync<object, TResponse>(HttpMethod.Get, url, null);
        }

        // Generic DELETE request
        public async Task<TResponse> DeleteAsync<TResponse>(string url)
        {
            return await SendRequestAsync<object, TResponse>(HttpMethod.Delete, url, null);
        }

        private async Task<TResponse> SendRequestAsync<TRequest, TResponse>(HttpMethod method, string url, TRequest requestData)
        {
            try
            {
                using var request = new HttpRequestMessage(method, url);

                if (requestData != null && (method == HttpMethod.Post || method == HttpMethod.Put))
                {
                    string jsonContent = JsonSerializer.Serialize(requestData, new JsonSerializerOptions
                    {
                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                    });

                    request.Content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                }
                using var response = await _httpClient.SendAsync(request);
                //response.EnsureSuccessStatusCode();
                var responseData = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"[DOTNET] Raw API Response: {responseData}");
                if (string.IsNullOrWhiteSpace(responseData))
                {
                    Console.WriteLine("[DOTNET] Warning: API response is empty.");
                    return default;
                }

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
                };

                try
                {
                    return JsonSerializer.Deserialize<TResponse>(responseData, options);
                }
                catch (JsonException jsonEx)
                {
                    Console.Error.WriteLine($"[DOTNET] JSON Deserialization Error: {jsonEx.Message}");
                    Console.Error.WriteLine($"[DOTNET] Response that caused the error: {responseData}");
                    throw;
                }
            }
            catch (HttpRequestException ex)
            {
                throw new HttpRequestException($"HTTP request failed: {ex.Message}", ex);
            }
            catch (JsonException ex)
            {
                throw new InvalidOperationException("Failed to deserialize JSON response.", ex);
            }
        }
    }
}
