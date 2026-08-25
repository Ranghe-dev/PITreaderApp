using PITreaderApp.Models;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;

namespace PITreaderApp.Services
{
    public class PitReaderClient
    {
        private readonly HttpClient _httpClient;

        public PitReaderClient()
        {
            var handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback =
                    HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
            };

            _httpClient = new HttpClient(handler);
        }

        public void SetToken(string token)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }
        public async Task<StatusResponse?> GetStatusAsync(string ip)
        {
            string url = $"https://{ip}/api/status/monitor";

            var response = await _httpClient.GetAsync(url);

            response.EnsureSuccessStatusCode();

            string json = await response.Content.ReadAsStringAsync();     

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            return JsonSerializer.Deserialize<StatusResponse>(json, options);
        }
        public async Task<DiagnosticLogResponse?> GetDiagnosticLogAsync(string ip)
        {
            string url = $"https://{ip}/api/diag/log";

            var response = await _httpClient.GetAsync(url);

            response.EnsureSuccessStatusCode();

            string json = await response.Content.ReadAsStringAsync();

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            return JsonSerializer.Deserialize<DiagnosticLogResponse>(json, options);
        }
    }
}
