using System.Net;
using System.Net.Http.Json;
using Moq;
using Moq.Protected;
using Polly;
using Polly.Registry;
using Polly.Retry;
using PV260.Client.BL;
using PV260.Common.Models;
using PV260.Tests.Common;

namespace PV260.Client.Tests;

public class ApiClientResilienceTests
{
    private readonly Mock<HttpMessageHandler> _httpMessageHandlerMock;
    private readonly ApiClient _apiClient;

    public ApiClientResilienceTests()
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
    public void ApiReturnsSuccess_ExecutesOnlyOnce()
    {
        new Given(_apiClient, _httpMessageHandlerMock)
            .MockedApiReturnsCorrectValues()
            .When()
            .ITryToGetAllReports()
            .Then()
            .RequestWasSentOnce()
            .NoExceptionWasThrown()
            .ActualReportsAreEqualToExpected();
    }
    
    [Fact]
    public void ApiReturnsError_ExecutesMultipleTimes()
    {
        new Given(_apiClient, _httpMessageHandlerMock)
            .MockedApiReturnsError()
            .When()
            .ITryToGetAllReports()
            .Then()
            .RequestWasSentMultipleTimes(4) // 3 retries + 1 original request
            .ExceptionWasThrown<AggregateException>();
    }
    
    [Fact]
    public void ApiTimeouts_ExecutesMultipleTimes()
    {
        new Given(_apiClient, _httpMessageHandlerMock)
            .MockedApiTimeouts()
            .When()
            .ITryToGetAllReports()
            .Then()
            .RequestWasSentMultipleTimes(4) // 3 retries + 1 original request
            .ExceptionWasThrown<AggregateException>();
    }

    #region Internals

    private class Counter(int value)
    {
        public int Value = value;
    }
    
    private class Given(ApiClient apiClient, Mock<HttpMessageHandler> httpMessageHandlerMock)
    {
        private readonly PaginatedResponse<ReportListModel> _expectedReports = new()
        {
            Items = new List<ReportListModel>
            {
                new()
                {
                    Name = "Test",
                    CreatedAt = new DateTime(2025, 4, 9, 12, 36, 22)
                }
            },
            PageSize = 10,
            TotalCount = 1
        };

        private readonly Counter _requestCount = new(0);

        public Given MockedApiReturnsCorrectValues()
        {
            httpMessageHandlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .Callback(() => _requestCount.Value++)
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = JsonContent.Create(_expectedReports)
                });

            return this;
        }
        
        public Given MockedApiReturnsError()
        {
            httpMessageHandlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .Callback(() => _requestCount.Value++)
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                });

            return this;
        }
        
        public Given MockedApiTimeouts()
        {
            httpMessageHandlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .Callback(() => _requestCount.Value++)
                .ThrowsAsync(new TimeoutException());

            return this;
        }

        public When When()
        {
            return new When(apiClient, _expectedReports, _requestCount);
        }
    }

    private class When(ApiClient apiClient, PaginatedResponse<ReportListModel> expectedReports, Counter requestCount)
    {
        private PaginatedResponse<ReportListModel>? _actualReports;
        private Exception? _exception;
        
        public When ITryToGetAllReports()
        {
            var paginationCursor = new PaginationCursor
            {
                PageSize = 10,
            };

            try
            {
                _actualReports = apiClient.GetAllReportsAsync(paginationCursor).Result;
            }
            catch (Exception ex)
            {
                _exception = ex;
            }

            return this;
        }

        public Then Then()
        {
            return new Then(expectedReports, _actualReports, _exception, requestCount);
        }
    }

    private class Then(PaginatedResponse<ReportListModel> expectedReports, PaginatedResponse<ReportListModel>? actualReports, Exception? exception, Counter requestCount)
    {
        public Then NoExceptionWasThrown()
        {
            Assert.Null(exception);
            return this;
        }
        
        public Then ExceptionWasThrown<TException>()
            where TException : Exception
        {
            Assert.IsType<TException>(exception);
            return this;
        }
        
        public Then ActualReportsAreEqualToExpected()
        {
            DeepAssert.Equal(expectedReports, actualReports);
            return this;
        }
        
        public Then RequestWasSentOnce()
        {
            Assert.Equal(1, requestCount.Value);
            return this;
        }
        
        public Then RequestWasSentMultipleTimes(int times)
        {
            Assert.Equal(times, requestCount.Value);
            return this;
        }
    }
    
    #endregion
}