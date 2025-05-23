using PV260.Client.Mock;
using PV260.Common.Models;

namespace PV260.Client.Tests;
public class ApiClientMockTests
{
    private readonly ApiClientMock _apiClient = new();

    [Fact]
    public async Task GetAllReportsAsync_ReturnsEmptyList_Initially()
    {
        var reports = await _apiClient.GetAllReportsAsync();

        Assert.False(reports.IsError);
        Assert.Empty(reports.Value);
    }

    [Fact]
    public async Task GenerateNewReportAsync_CreatesNewReport()
    {
        var report = await _apiClient.GenerateNewReportAsync();
        
        Assert.False(report.IsError);
        Assert.NotNull(report.Value);
        Assert.NotEqual(Guid.Empty, report.Value.Id);
        Assert.NotNull(report.Value.Name);
        Assert.NotEmpty(report.Value.Records);
    }

    [Fact]
    public async Task GetLatestReportAsync_ReturnsLatestReport()
    {
        await _apiClient.GenerateNewReportAsync();
        var report = await _apiClient.GenerateNewReportAsync();

        var latest = await _apiClient.GetLatestReportAsync();
        Assert.False(latest.IsError);
        Assert.NotNull(latest.Value);
        Assert.Equal(report.Value.Id, latest.Value.Id);
    }

    [Fact]
    public async Task GetReportByIdAsync_ReturnsCorrectReport()
    {
        var report = await _apiClient.GenerateNewReportAsync();
        var retrieved = await _apiClient.GetReportByIdAsync(report.Value.Id);
        
        Assert.False(report.IsError);
        Assert.False(retrieved.IsError);
        Assert.NotNull(retrieved.Value);
        Assert.Equal(report.Value.Id, retrieved.Value.Id);
    }

    [Fact]
    public async Task DeleteReportAsync_RemovesReport()
    {
        var report = await _apiClient.GenerateNewReportAsync();
        await _apiClient.DeleteReportAsync(report.Value.Id);
        var retrieved = await _apiClient.GetReportByIdAsync(report.Value.Id);
        Assert.Null(retrieved.Value);
    }

    [Fact]
    public async Task DeleteAllReportsAsync_ClearsAllReports()
    {
        await _apiClient.GenerateNewReportAsync();
        await _apiClient.GenerateNewReportAsync();
        await _apiClient.DeleteAllReportsAsync();
        var response = await _apiClient.GetAllReportsAsync();
        Assert.Empty(response.Value);
    }

    [Fact]
    public async Task SendReportAsync_DoesNotThrow()
    {
        var report = await _apiClient.GenerateNewReportAsync();
        var exception = await Record.ExceptionAsync(() => _apiClient.SendReportAsync(report.Value.Id));
        Assert.Null(exception);
    }

    [Fact]
    public async Task GetAllEmailsAsync_ReturnsAllEmails()
    {
        // Act
        var emails = await _apiClient.GetAllEmailsAsync();

        Assert.Empty(emails.Value);

        var emailRecipient = new EmailRecipientModel
        {
            EmailAddress = "test@example.com",
            CreatedAt = DateTime.UtcNow
        };

        await _apiClient.AddEmailAsync(emailRecipient);

        emails = await _apiClient.GetAllEmailsAsync();

        // Assert
        Assert.Single(emails.Value);

        Assert.Equal("test@example.com", emails.Value.First().EmailAddress);
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
        await _apiClient.AddEmailAsync(emailRecipient);

        // Assert
        var paginatedEmails = await _apiClient.GetAllEmailsAsync();
        Assert.Contains(paginatedEmails.Value, e => e.EmailAddress == "test@example.com");
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

        await _apiClient.AddEmailAsync(emailRecipient);

        // Act
        await _apiClient.DeleteEmailAsync("test@example.com");

        // Assert
        var paginatedEmails = await _apiClient.GetAllEmailsAsync();
        Assert.DoesNotContain(paginatedEmails.Value, e => e.EmailAddress == "test@example.com");
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

        await _apiClient.AddEmailAsync(emailRecipient1);
        await _apiClient.AddEmailAsync(emailRecipient2);

        // Act
        await _apiClient.DeleteAllEmailsAsync();

        // Assert
        var paginatedEmails = await _apiClient.GetAllEmailsAsync();

        Assert.Empty(paginatedEmails.Value);
    }
}
