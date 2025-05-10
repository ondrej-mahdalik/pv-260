using PV260.Client.Mock;
using PV260.Common.Models;

namespace PV260.Client.Tests;
public class ApiClientMockTests
{
    private readonly ApiClientMock _apiClient = new();

    [Fact]
    public async Task GetAllReportsAsync_ReturnsEmptyList_Initially()
    {
        var paginationCursor = new PaginationCursor
        {
            PageSize = 10,
        };

        var paginatedReports = await _apiClient.GetAllReportsAsync(paginationCursor);

        Assert.Empty(paginatedReports.Items);
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
        await _apiClient.GenerateNewReportAsync();
        var report = await _apiClient.GenerateNewReportAsync();

        var latest = await _apiClient.GetLatestReportAsync();
        Assert.NotNull(latest);
        Assert.Equal(report.Id, latest.Id);
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
        var paginationCursor = new PaginationCursor
        {
            PageSize = 10,
        };

        await _apiClient.GenerateNewReportAsync();
        await _apiClient.GenerateNewReportAsync();
        await _apiClient.DeleteAllReportsAsync();
        var paginatedReports = await _apiClient.GetAllReportsAsync(paginationCursor);
        Assert.Empty(paginatedReports.Items);
    }

    [Fact]
    public async Task SendReportAsync_DoesNotThrow()
    {
        var report = await _apiClient.GenerateNewReportAsync();
        var exception = await Record.ExceptionAsync(() => _apiClient.SendReportAsync(report.Id));
        Assert.Null(exception);
    }

    [Fact]
    public async Task GetAllEmailsAsync_ReturnsAllEmails()
    {
        // Act
        var paginationCursor = new PaginationCursor
        {
            PageSize = 10,
        };

        var paginatedEmails = await _apiClient.GetAllEmailsAsync(paginationCursor);

        Assert.Empty(paginatedEmails.Items);

        var emailRecipient = new EmailRecipientModel
        {
            EmailAddress = "test@example.com",
            CreatedAt = DateTime.UtcNow
        };

        await _apiClient.AddEmailAsync(emailRecipient);

        paginatedEmails = await _apiClient.GetAllEmailsAsync(paginationCursor);

        // Assert
        Assert.Single(paginatedEmails.Items);

        Assert.Equal("test@example.com", paginatedEmails.Items.First().EmailAddress);
    }

    [Fact]
    public async Task AddEmailAsync_AddsEmail()
    {
        // Arrange
        var paginationCursor = new PaginationCursor
        {
            PageSize = 10,
        };

        var emailRecipient = new EmailRecipientModel
        {
            EmailAddress = "test@example.com",
            CreatedAt = DateTime.UtcNow
        };

        // Act
        await _apiClient.AddEmailAsync(emailRecipient);

        // Assert
        var paginatedEmails = await _apiClient.GetAllEmailsAsync(paginationCursor);
        Assert.Contains(paginatedEmails.Items, e => e.EmailAddress == "test@example.com");
    }

    [Fact]
    public async Task DeleteEmailAsync_RemovesSpecificEmail()
    {
        // Arrange
        var paginationCursor = new PaginationCursor
        {
            PageSize = 10,
        };

        var emailRecipient = new EmailRecipientModel
        {
            EmailAddress = "test@example.com",
            CreatedAt = DateTime.UtcNow
        };

        await _apiClient.AddEmailAsync(emailRecipient);

        // Act
        await _apiClient.DeleteEmailAsync("test@example.com");

        // Assert
        var paginatedEmails = await _apiClient.GetAllEmailsAsync(paginationCursor);
        Assert.DoesNotContain(paginatedEmails.Items, e => e.EmailAddress == "test@example.com");
    }

    [Fact]
    public async Task DeleteAllEmailsAsync_RemovesAllEmails()
    {
        // Arrange
        var paginationCursor = new PaginationCursor
        {
            PageSize = 10,
        };

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
        var paginatedEmails = await _apiClient.GetAllEmailsAsync(paginationCursor);

        Assert.Empty(paginatedEmails.Items);
    }
}
