using Moq;
using PV260.Client.BL;
using PV260.Client.Mock;
using PV260.Common.Models;
using Xunit;

namespace PV260.Client.Tests;

public class ApiClientMockTests
{
    private readonly ApiClientMock _apiClient;
    private readonly Mock<HttpClient> _httpClientMock;

    public ApiClientMockTests()
    {
        _httpClientMock = new Mock<HttpClient>();
        _apiClient = new ApiClientMock(_httpClientMock.Object);
    }

    [Fact]
    public async Task GetAllReportsAsync_ReturnsEmptyList_Initially()
    {
        var reports = await _apiClient.GetAllReportsAsync();
        Assert.Empty(reports);
    }

    [Fact]
    public async Task GenerateNewReportAsync_CreatesNewReport()
    {
        var report = await _apiClient.GenerateNewReportAsync();
        Assert.NotNull(report);
        Assert.NotEqual(Guid.Empty, report.Id);
        Assert.NotNull(report.Name);
        Assert.NotEmpty(report.Records);
    }

    [Fact]
    public async Task GetLatestReportAsync_ReturnsLatestReport()
    {
        var report1 = await _apiClient.GenerateNewReportAsync();
        var report2 = await _apiClient.GenerateNewReportAsync();

        var latest = await _apiClient.GetLatestReportAsync();
        Assert.NotNull(latest);
        Assert.Equal(report2.Id, latest.Id);
    }

    [Fact]
    public async Task GetReportByIdAsync_ReturnsCorrectReport()
    {
        var report = await _apiClient.GenerateNewReportAsync();
        var retrieved = await _apiClient.GetReportByIdAsync(report.Id);
        Assert.NotNull(retrieved);
        Assert.Equal(report.Id, retrieved.Id);
    }

    [Fact]
    public async Task DeleteReportAsync_RemovesReport()
    {
        var report = await _apiClient.GenerateNewReportAsync();
        await _apiClient.DeleteReportAsync(report.Id);
        var retrieved = await _apiClient.GetReportByIdAsync(report.Id);
        Assert.Null(retrieved);
    }

    [Fact]
    public async Task DeleteAllReportsAsync_ClearsAllReports()
    {
        await _apiClient.GenerateNewReportAsync();
        await _apiClient.GenerateNewReportAsync();
        await _apiClient.DeleteAllReportsAsync();
        var reports = await _apiClient.GetAllReportsAsync();
        Assert.Empty(reports);
    }

    [Fact]
    public async Task SendReportAsync_DoesNotThrow()
    {
        var report = await _apiClient.GenerateNewReportAsync();
        var exception = await Record.ExceptionAsync(() => _apiClient.SendReportAsync(report.Id));
        Assert.Null(exception);
    }
}
