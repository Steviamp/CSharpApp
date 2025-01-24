

using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace CSharpApp.Infrastructure.Clients
{
    public interface IPlatziApiClient
    {
        Task<string> GetDataAsync(string endpoint);
    }

    public class PlatziApiClient : IPlatziApiClient
    {
        private readonly HttpClient _httpClient;
        private readonly RestApiSettings _restApiSettings;
        private string _token;
        private DateTime _tokenExpiration;

        public PlatziApiClient(HttpClient httpClient, RestApiSettings restApiSettings)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _restApiSettings = restApiSettings;
        }
        public async Task<string> GetDataAsync(string endpoint)
        {
            var token = await GetTokenAsync();
            var request = new HttpRequestMessage(HttpMethod.Get, endpoint);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _token);

            var response = await _httpClient.SendAsync(request);

            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }

        private async Task<string> GetTokenAsync()
        {
            if (!string.IsNullOrEmpty(_token) && _tokenExpiration > DateTime.UtcNow) 
            { 
                return _token; 
            }

            var loginRequest = new
            {
                username = _restApiSettings.Username,
                password = _restApiSettings.Password
            };

            var content = new StringContent(JsonSerializer.Serialize(loginRequest), System.Text.Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(_restApiSettings.Auth, content);

            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();
            var tokenResponse = JsonSerializer.Deserialize<TokenResponse>(responseContent);
            _tokenExpiration = DateTime.UtcNow.AddDays(19).AddHours(23);

            return tokenResponse?.AccessToken ?? throw new InvalidOperationException("Token response is invalid.");
        }

        private class TokenResponse
        {
            [JsonPropertyName("access_token")]
            public string? AccessToken { get; set; }

            [JsonPropertyName("refresh_token")]
            public string? RefreshToken { get; set; }
        }
    }
}


