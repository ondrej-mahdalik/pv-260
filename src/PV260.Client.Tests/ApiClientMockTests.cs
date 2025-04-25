using Moq;
using Moq.Protected;
using PV260.Client.BL;
using PV260.Client.Mock;
using PV260.Common.Models;
using System.Net;
using System.Net.Http.Json;

namespace PV260.Client.Tests;

public class ApiClientMockTests
{
    private readonly IApiClient _client;

    public ApiClientMockTests()
    {
        // Provide a dummy HttpClient to satisfy the constructor requirement
        var httpClient = new HttpClient
        {
            BaseAddress = new Uri("https://localhost:7288")
        };
        _client = new ApiClientMock();
    }
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

    [Fact]
    public async Task GetAllEmailsAsync_ReturnsAllEmails()
    {
        // Act
        var emails = await _client.GetAllEmailsAsync();
        Assert.Empty(emails);

        var emailRecipient = new EmailRecipientModel
        {
            EmailAddress = "test@example.com",
            CreatedAt = DateTime.UtcNow
        };

        await _client.AddEmailAsync(emailRecipient);

        emails = await _client.GetAllEmailsAsync();

        // Assert
        Assert.Single(emails);
        Assert.Equal("test@example.com", emails.First().EmailAddress);
    }

    [Fact]
    public async Task AddEmailAsync_AddsEmail()
    {
        // Arrange
        var emailRecipient = new EmailRecipientModel
        {
            EmailAddress = "test@example.com",
            CreatedAt = DateTime.UtcNow
        };

        // Act
        await _client.AddEmailAsync(emailRecipient);

        // Assert
        var emails = await _client.GetAllEmailsAsync();
        Assert.Contains(emails, e => e.EmailAddress == "test@example.com");
    }

    [Fact]
    public async Task DeleteEmailAsync_RemovesSpecificEmail()
    {
        // Arrange
        var emailRecipient = new EmailRecipientModel
        {
            EmailAddress = "test@example.com",
            CreatedAt = DateTime.UtcNow
        };

        await _client.AddEmailAsync(emailRecipient);

        // Act
        await _client.DeleteEmailAsync("test@example.com");

        // Assert
        var emails = await _client.GetAllEmailsAsync();
        Assert.DoesNotContain(emails, e => e.EmailAddress == "test@example.com");
    }

    [Fact]
    public async Task DeleteAllEmailsAsync_RemovesAllEmails()
    {
        // Arrange
        var emailRecipient1 = new EmailRecipientModel
        {
            EmailAddress = "test1@example.com",
            CreatedAt = DateTime.UtcNow
        };

        var emailRecipient2 = new EmailRecipientModel
        {
            EmailAddress = "test2@example.com",
            CreatedAt = DateTime.UtcNow
        };

        await _client.AddEmailAsync(emailRecipient1);
        await _client.AddEmailAsync(emailRecipient2);

        // Act
        await _client.DeleteAllEmailsAsync();

        // Assert
        var emails = await _client.GetAllEmailsAsync();
        Assert.Empty(emails);
    }
}
