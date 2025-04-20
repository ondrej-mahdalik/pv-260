using PV260.Client.BL;
using PV260.Client.Mock;

namespace PV260.Client.Tests;

public class ApiClientMockTests
{
    private readonly IApiClient _client = new ApiClientMock();
    
    [Fact]
    public async Task GetAllReportsAsync_ReturnsAllReports()
    {
        var reports = await _client.GetAllReportsAsync();
        Assert.Empty(reports);
    }

    [Fact]
    public async Task GetReportByIdAsync_ReturnsReport_WhenReportExists()
    {
        var newReport = await _client.GenerateNewReportAsync();
        var report = await _client.GetReportByIdAsync(newReport.Id);
        Assert.Equal(newReport.Id, report?.Id);
    }

    [Fact]
    public async Task GetReportByIdAsync_ReturnsNull_WhenReportDoesNotExist()
    {
        var report = await _client.GetReportByIdAsync(Guid.NewGuid());
        Assert.Null(report);
    }

    [Fact]
    public async Task GetLatestReportAsync_ReturnsLatestReport()
    {
        var newReport = await _client.GenerateNewReportAsync();
        var latestReport = await _client.GetLatestReportAsync();
        Assert.Equal(newReport.Id, latestReport?.Id);
    }

    [Fact]
    public async Task GenerateNewReportAsync_AddsNewReport()
    {
        var newReport = await _client.GenerateNewReportAsync();
        var reports = await _client.GetAllReportsAsync();
        Assert.Contains(reports, r => r.Id == newReport.Id);
    }

    [Fact]
    public async Task DeleteReportAsync_RemovesReport_WhenReportExists()
    {
        var newReport = await _client.GenerateNewReportAsync();
        await _client.DeleteReportAsync(newReport.Id);
        var reports = await _client.GetAllReportsAsync();
        Assert.DoesNotContain(reports, r => r.Id == newReport.Id);
    }

    [Fact]
    public async Task DeleteAllReportsAsync_RemovesAllReports()
    {
        await _client.GenerateNewReportAsync();
        await _client.DeleteAllReportsAsync();
        var reports = await _client.GetAllReportsAsync();
        Assert.Empty(reports);
    }
}
