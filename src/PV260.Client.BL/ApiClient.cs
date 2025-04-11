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

        public async Task<IEnumerable<ReportListModel>> GetAllReportsAsync()
        {
            return await _httpClient.GetFromJsonAsync<IEnumerable<ReportListModel>>("Report") ?? new List<ReportListModel>();
        }

        public async Task<ReportListModel> GetReportByIdAsync(Guid id)
        {
            var report = await _httpClient.GetFromJsonAsync<ReportListModel>($"Report/{id}");
            return report ?? throw new InvalidOperationException("Report not found");
        }

        public async Task<ReportListModel> GetLatestReportAsync()
        {
            var report = await _httpClient.GetFromJsonAsync<ReportListModel>("Report/latest");
            return report ?? throw new InvalidOperationException("Latest report not found");
        }

        public async Task<ReportListModel> GenerateNewReportAsync()
        {
            var response = await _httpClient.PostAsJsonAsync("Report/generate", new { });
            response.EnsureSuccessStatusCode();
            var report = await response.Content.ReadFromJsonAsync<ReportListModel>();
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
    }
}
