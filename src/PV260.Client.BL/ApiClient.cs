using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using PV260.Common.Models;

namespace PV260.Client.BL
{
    
    public class ApiClient : IApiClient
    {
        private readonly HttpClient _httpClient;

        public ApiClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IEnumerable<ReportModel>> GetAllReportsAsync()
        {
            return await _httpClient.GetFromJsonAsync<IEnumerable<ReportModel>>("Report") ?? new List<ReportModel>();
        }

        public async Task<ReportModel> GetReportByIdAsync(Guid id)
        {
            var report = await _httpClient.GetFromJsonAsync<ReportModel>($"Report/{id}");
            return report ?? throw new InvalidOperationException("Report not found");
        }

        public async Task<ReportModel> GetLatestReportAsync()
        {
            var report = await _httpClient.GetFromJsonAsync<ReportModel>("Report/latest");
            return report ?? throw new InvalidOperationException("Latest report not found");
        }

        public async Task<ReportModel> GenerateNewReportAsync()
        {
            var response = await _httpClient.PostAsJsonAsync("Report/generate", new { });
            response.EnsureSuccessStatusCode();
            var report = await response.Content.ReadFromJsonAsync<ReportModel>();
            return report ?? throw new InvalidOperationException("Failed to generate new report");
        }

        public async Task DeleteReportAsync(Guid id)
        {
            var response = await _httpClient.DeleteAsync($"Report/{id}");
            response.EnsureSuccessStatusCode();
        }

        public async Task DeleteAllReportsAsync()
        {
            var response = await _httpClient.DeleteAsync("Report/all");
            response.EnsureSuccessStatusCode();
        }

        public async Task SendReportAsync(Guid id)
        {
            var response = await _httpClient.PostAsJsonAsync($"Report/{id}/send", new { });
            response.EnsureSuccessStatusCode();
        }

        public async Task<SettingsModel> GetSettingsAsync()
        {
            var settings = await _httpClient.GetFromJsonAsync<SettingsModel>("Configuration");
            return settings ?? throw new InvalidOperationException("Settings not found");
        }

        public async Task<SettingsModel> UpdateSettingsAsync(SettingsModel settings)
        {
            var response = await _httpClient.PostAsJsonAsync("Configuration", settings);
            response.EnsureSuccessStatusCode();
            var updatedSettings = await response.Content.ReadFromJsonAsync<SettingsModel>();
            return updatedSettings ?? throw new InvalidOperationException("Failed to update settings");
        }
    }
}
