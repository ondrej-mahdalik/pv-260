using PV260.Client.Mock;
using PV260.Common.Models;

namespace PV260.Client.Tests;

public class ApiClientMockTests
{
    [Fact]
    public async Task GetAllReportsAsync_ReturnsAllReports()
    {
        var client = new ApiClientMock();
        var reports = await client.GetAllReportsAsync();
        Assert.Empty(reports);
    }

    [Fact]
    public async Task GetReportByIdAsync_ReturnsReport_WhenReportExists()
    {
        var client = new ApiClientMock();
        var newReport = await client.GenerateNewReportAsync();
        var report = await client.GetReportByIdAsync(newReport.Id);
        Assert.Equal(newReport.Id, report.Id);
    }

    [Fact]
    public async Task GetReportByIdAsync_ThrowsException_WhenReportDoesNotExist()
    {
        var client = new ApiClientMock();
        await Assert.ThrowsAsync<InvalidOperationException>(() => client.GetReportByIdAsync(Guid.NewGuid()));
    }

    [Fact]
    public async Task GetLatestReportAsync_ReturnsLatestReport()
    {
        var client = new ApiClientMock();
        var newReport = await client.GenerateNewReportAsync();
        var latestReport = await client.GetLatestReportAsync();
        Assert.Equal(newReport.Id, latestReport.Id);
    }

    [Fact]
    public async Task GenerateNewReportAsync_AddsNewReport()
    {
        var client = new ApiClientMock();
        var newReport = await client.GenerateNewReportAsync();
        var reports = await client.GetAllReportsAsync();
        Assert.Contains(reports, r => r.Id == newReport.Id);
    }

    [Fact]
    public async Task DeleteReportAsync_RemovesReport_WhenReportExists()
    {
        var client = new ApiClientMock();
        var newReport = await client.GenerateNewReportAsync();
        await client.DeleteReportAsync(newReport.Id);
        var reports = await client.GetAllReportsAsync();
        Assert.DoesNotContain(reports, r => r.Id == newReport.Id);
    }

    [Fact]
    public async Task DeleteAllReportsAsync_RemovesAllReports()
    {
        var client = new ApiClientMock();
        await client.GenerateNewReportAsync();
        await client.DeleteAllReportsAsync();
        var reports = await client.GetAllReportsAsync();
        Assert.Empty(reports);
    }

    [Fact]
    public async Task GetSettingsAsync_ReturnsCurrentSettings()
    {
        var client = new ApiClientMock();
        var settings = await client.GetSettingsAsync();
        Assert.NotNull(settings);
    }

    [Fact]
    public async Task UpdateSettingsAsync_UpdatesSettings()
    {
        var client = new ApiClientMock();
        var newSettings = new SettingsModel("59 22 * * 0", 7, true);
        var updatedSettings = await client.UpdateSettingsAsync(newSettings);
        Assert.Equal(newSettings, updatedSettings);
    }
}