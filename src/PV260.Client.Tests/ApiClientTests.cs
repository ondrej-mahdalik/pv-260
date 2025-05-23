using PV260.Client.BL;
using PV260.Common.Models;
using System.Net;
using System.Net.Http.Json;
using Moq;
using Moq.Protected;
using Polly;
using Polly.Registry;
using Polly.Retry;

namespace PV260.Client.Tests;

public class ApiClientTests
{
    private readonly Mock<HttpMessageHandler> _httpMessageHandlerMock;
    private readonly ApiClient _apiClient;

    public ApiClientTests()
    {
        _httpMessageHandlerMock = new Mock<HttpMessageHandler>();
            
        var httpClient = new HttpClient(_httpMessageHandlerMock.Object)
        {
            BaseAddress = new Uri("https://localhost:7288")
        };
            
        var resiliencePipeline = new ResiliencePipelineBuilder()
            .AddRetry(new RetryStrategyOptions
            {
                MaxRetryAttempts = 3, Delay = TimeSpan.FromMilliseconds(100)
            })
            .AddTimeout(TimeSpan.FromSeconds(10))
            .Build();

        var resiliencePipelineProvider = new Mock<ResiliencePipelineProvider<string>>();
        resiliencePipelineProvider.Setup(x => x.GetPipeline(ApiClient.DefaultApiClientPipeline))
            .Returns(resiliencePipeline);
            
        _apiClient = new ApiClient(httpClient, resiliencePipelineProvider.Object);
    }

    [Fact]
    public async Task GetAllReportsAsync_ReturnsReports()
    {
        // Arrange
        var paginationCursor = new PaginationCursor
        {
            PageSize = 10,
        };

        var expectedReports = new PaginatedResponse<ReportListModel>
        {
            Items = new List<ReportListModel>()
            { new()
                {
                    Name = "Test",
                    CreatedAt = new DateTime(2025, 4, 9, 12, 36, 22)
                }
            },
            PageSize = 10,
            TotalCount = 1
        };

        _httpMessageHandlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = JsonContent.Create(expectedReports)
            });

        // Act
        var reports = await _apiClient.GetAllReportsAsync(paginationCursor);

        // Assert
        Assert.False(reports.IsError);
        Assert.Single(reports.Value.Items);
    }

    [Fact]
    public async Task GetReportByIdAsync_ReturnsReport()
    {
        // Arrange
        var expectedReport = new ReportDetailModel
        {
            Name = "Test",
            CreatedAt = new DateTime(2025, 4, 9, 12, 36, 22),
            Records = [new ReportRecordModel
            {
                CompanyName = "Test Company",
                Ticker = "Test Ticker",
                Weight = 0.5,
                NumberOfShares = 100,
                SharesChangePercentage = 0.1
            }]
        };
        _httpMessageHandlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = JsonContent.Create(expectedReport)
            });

        // Act
        var report = await _apiClient.GetReportByIdAsync(Guid.NewGuid());

        // Assert
        Assert.NotNull(report);
    }

    [Fact]
    public async Task GetLatestReportAsync_ReturnsLatestReport()
    {
        // Arrange
        var expectedReport = new ReportDetailModel
        {
            Name = "Test",
            CreatedAt = new DateTime(2025, 4, 9, 12, 36, 22),
            Records = [new ReportRecordModel
            {
                CompanyName = "Test Company",
                Ticker = "Test Ticker",
                Weight = 0.5,
                NumberOfShares = 100,
                SharesChangePercentage = 0.1
            }]
        };
        _httpMessageHandlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = JsonContent.Create(expectedReport)
            });

        // Act
        var report = await _apiClient.GetLatestReportAsync();

        // Assert
        Assert.NotNull(report);
    }

    [Fact]
    public async Task GenerateNewReportAsync_ReturnsNewReport()
    {
        // Arrange
        var expectedReport = new ReportDetailModel
        {
            Name = "Test",
            CreatedAt = new DateTime(2025, 4, 9, 12, 36, 22),
            Records = [new ReportRecordModel
            {
                CompanyName = "Test Company",
                Ticker = "Test Ticker",
                Weight = 0.5,
                NumberOfShares = 100,
                SharesChangePercentage = 0.1
            }]
        };
        _httpMessageHandlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = JsonContent.Create(expectedReport)
            });

        // Act
        var report = await _apiClient.GenerateNewReportAsync();

        // Assert
        Assert.NotNull(report);
    }

    [Fact]
    public async Task DeleteReportAsync_DeletesReport()
    {
        // Arrange
        _httpMessageHandlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.NoContent
            });

        // Act
        await _apiClient.DeleteReportAsync(Guid.NewGuid());

        // Assert
        _httpMessageHandlerMock.Protected().Verify(
            "SendAsync",
            Times.Once(),
            ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Delete),
            ItExpr.IsAny<CancellationToken>());
    }

    [Fact]
    public async Task DeleteAllReportsAsync_DeletesAllReports()
    {
        // Arrange
        _httpMessageHandlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.NoContent
            });

        // Act
        await _apiClient.DeleteAllReportsAsync();

        // Assert
        _httpMessageHandlerMock.Protected().Verify(
            "SendAsync",
            Times.Once(),
            ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Delete),
            ItExpr.IsAny<CancellationToken>());
    }

    [Fact]
    public async Task SendReportAsync_SendsReport()
    {
        // Arrange
        _httpMessageHandlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK
            });

        // Act
        await _apiClient.SendReportAsync(Guid.NewGuid());

        // Assert
        _httpMessageHandlerMock.Protected().Verify(
            "SendAsync",
            Times.Once(),
            ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Post),
            ItExpr.IsAny<CancellationToken>());
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

        _httpMessageHandlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Post),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK
            });

        // Act
        await _apiClient.AddEmailAsync(emailRecipient);

        // Assert
        _httpMessageHandlerMock.Protected().Verify(
            "SendAsync",
            Times.Once(),
            ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Post),
            ItExpr.IsAny<CancellationToken>());
    }

    [Fact]
    public async Task DeleteAllEmailsAsync_RemovesAllEmails()
    {
        // Arrange
        _httpMessageHandlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Delete),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.NoContent
            });

        // Act
        await _apiClient.DeleteAllEmailsAsync();

        // Assert
        _httpMessageHandlerMock.Protected().Verify(
            "SendAsync",
            Times.Once(),
            ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Delete),
            ItExpr.IsAny<CancellationToken>());
    }
}