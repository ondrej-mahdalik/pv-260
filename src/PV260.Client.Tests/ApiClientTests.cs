using Moq;
using Moq.Protected;
using PV260.Client.BL;
using PV260.Common.Models;
using System.Net;
using System.Net.Http.Json;

namespace PV260.Client.Tests
{
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
            _apiClient = new ApiClient(httpClient);
        }

        [Fact]
        public async Task GetAllReportsAsync_ReturnsReports()
        {
            // Arrange
            var expectedReports = new List<ReportListModel>
            {
                new()
                {
                    Name = "Test",
                    CreatedAt = new DateTime(2025, 4, 9, 12, 36, 22)
                }
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
            var reports = await _apiClient.GetAllReportsAsync();

            // Assert
            Assert.NotNull(reports);
            Assert.Single(reports);
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

        // [Fact]
        // public async Task GetSettingsAsync_ReturnsSettings()
        // {
        //     // Arrange
        //     var expectedSettings = new SettingModel("0 0 * * *", 15, false);
        //     _httpMessageHandlerMock.Protected()
        //         .Setup<Task<HttpResponseMessage>>(
        //             "SendAsync",
        //             ItExpr.IsAny<HttpRequestMessage>(),
        //             ItExpr.IsAny<CancellationToken>())
        //         .ReturnsAsync(new HttpResponseMessage
        //         {
        //             StatusCode = HttpStatusCode.OK,
        //             Content = JsonContent.Create(expectedSettings)
        //         });
        //
        //     // Act
        //     var settings = await _apiClient.GetSettingsAsync();
        //
        //     // Assert
        //     Assert.NotNull(settings);
        //     Assert.Equal(expectedSettings.ReportGenerationCron, settings.ReportGenerationCron);
        // }
        //
        // [Fact]
        // public async Task UpdateSettingsAsync_UpdatesSettings()
        // {
        //     // Arrange
        //     var newSettings = new SettingModel("0 0 * * *", 15, false);
        //     _httpMessageHandlerMock.Protected()
        //         .Setup<Task<HttpResponseMessage>>(
        //             "SendAsync",
        //             ItExpr.IsAny<HttpRequestMessage>(),
        //             ItExpr.IsAny<CancellationToken>())
        //         .ReturnsAsync(new HttpResponseMessage
        //         {
        //             StatusCode = HttpStatusCode.OK,
        //             Content = JsonContent.Create(newSettings)
        //         });
        //
        //     // Act
        //     var updatedSettings = await _apiClient.UpdateSettingsAsync(newSettings);
        //
        //     // Assert
        //     Assert.NotNull(updatedSettings);
        //     Assert.Equal(newSettings.ReportGenerationCron, updatedSettings.ReportGenerationCron);
        // }
    }
}