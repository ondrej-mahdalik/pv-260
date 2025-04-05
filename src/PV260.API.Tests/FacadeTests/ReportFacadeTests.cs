using Microsoft.EntityFrameworkCore;
using PV260.API.BL.Facades;
using PV260.API.DAL.Entities;
using PV260.Common.Models;
using PV260.Common.Tests;

namespace PV260.API.Tests.FacadeTests;

public class ReportFacadeTests : FacadeTestBase
{
    private readonly List<ReportDetailModel> _reports =
    [
        new()
        {
            Name = "Test Report 1",
            CreatedAt = new DateTime(2025, 4, 16, 5, 46, 01),
            Records = new List<ReportRecordModel>
            {
                new()
                {
                    CompanyName = "Test Company 1",
                    Ticker = "Test Ticker 2",
                    Weight = 2.5,
                    NumberOfShares = 200,
                    SharesChangePercentage = 0.2
                }
            }
        },
        new()
        {
            Name = "Test Report 2",
            CreatedAt = new DateTime(2025, 4, 16, 5, 51, 23),
            Records = new List<ReportRecordModel>
            {
                new()
                {
                    CompanyName = "Test Company 2",
                    Ticker = "Test Ticker 2",
                    Weight = 3.5,
                    NumberOfShares = 300,
                    SharesChangePercentage = 0.3
                }
            }
        }
    ];

    private List<ReportListModel> ListReports
        => _reports.Select(x => ReportMapper.ToListModel(ReportMapper.ToEntity(x))).ToList();

    [Fact]
    public async Task GetAllReportsAsync_ReturnsAllReports()
    {
        // Arrange
        await AddMockedEntitiesAsync();
        
        // Act
        var actualReports = await ReportFacadeSut.GetAsync();
        
        // Assert
        Assert.Equal(_reports.Count, actualReports.Count);
        foreach (var expectedReport in ListReports)
        {
            DeepAssert.Contains(expectedReport, actualReports);
        }
    }
    
    [Fact]
    public async Task GetReportByIdAsync_ReturnsCorrectReport()
    {
        // Arrange
        await AddMockedEntitiesAsync();
        var reportToGet = _reports.First();
        
        // Act
        var actualReport = await ReportFacadeSut.GetAsync(reportToGet.Id);
        
        // Assert
        Assert.NotNull(actualReport);
        DeepAssert.Equal(reportToGet, actualReport);
    }
    
    [Fact]
    public async Task GetReportByIdAsync_ReturnsNull_WhenReportDoesNotExist()
    {
        // Arrange
        await AddMockedEntitiesAsync();
        
        // Act
        var actualReport = await ReportFacadeSut.GetAsync(Guid.NewGuid());
        
        // Assert
        Assert.Null(actualReport);
    }
    
    [Fact]
    public async Task SaveReportAsync_AddsNewReportWithRecords()
    {
        // Arrange
        var newReport = new ReportDetailModel
        {
            Name = "New Report",
            CreatedAt = DateTime.UtcNow,
            Records = new List<ReportRecordModel>
            {
                new()
                {
                    CompanyName = "New Company",
                    Ticker = "New Ticker",
                    Weight = 1.0,
                    NumberOfShares = 100,
                    SharesChangePercentage = 0.1
                }
            }
        };
        
        // Act
        await ReportFacadeSut.SaveAsync(newReport);
        
        // Assert
        await using var uow = UnitOfWorkFactory.Create();
        var reportRepository = uow.GetRepository<ReportEntity>();
        var actualReportEntity = await reportRepository.Get()
            .FirstOrDefaultAsync(x => x.Name == newReport.Name);
        
        Assert.NotNull(actualReportEntity);
        DeepAssert.Equal(newReport, ReportMapper.ToDetailModel(actualReportEntity));
    }
    
    [Fact]
    public async Task SaveReportAsync_UpdatesExistingReportWithRecords()
    {
        // Arrange
        await AddMockedEntitiesAsync();
        var reportToUpdate = _reports.First();
        reportToUpdate.Name = "Updated Report";
        reportToUpdate.Records.Add(new ReportRecordModel
        {
            CompanyName = "Updated Company",
            Ticker = "Updated Ticker",
            Weight = 2.0,
            NumberOfShares = 200,
            SharesChangePercentage = 0.2
        });
        
        // Act
        await ReportFacadeSut.SaveAsync(reportToUpdate);
        
        // Assert
        await using var uow = UnitOfWorkFactory.Create();
        var reportRepository = uow.GetRepository<ReportEntity>();
        var actualReportEntity = await reportRepository.Get()
            .FirstOrDefaultAsync(x => x.Name == reportToUpdate.Name);
        
        Assert.NotNull(actualReportEntity);
        DeepAssert.Equal(reportToUpdate, ReportMapper.ToDetailModel(actualReportEntity));
    }

    [Fact]
    public async Task SaveReportAsync_UpdatesExistingRecord()
    {
        // Arrange
        await AddMockedEntitiesAsync();
        var reportToUpdate = _reports.First();
        reportToUpdate.Records.First().CompanyName = "Updated Company";
        
        // Act
        await ReportFacadeSut.SaveAsync(reportToUpdate);
        
        // Assert
        await using var uow = UnitOfWorkFactory.Create();
        var reportRepository = uow.GetRepository<ReportEntity>();
        var actualReportEntity = await reportRepository.Get()
            .FirstOrDefaultAsync(x => x.Id == reportToUpdate.Id);
        
        Assert.NotNull(actualReportEntity);
        DeepAssert.Equal(reportToUpdate, ReportMapper.ToDetailModel(actualReportEntity));
    }
    
    [Fact]
    public async Task DeleteReportAsync_DeletesExistingReportIncludingReports()
    {
        // Arrange
        await AddMockedEntitiesAsync();
        var reportToDelete = _reports.First();
        
        // Act
        await ReportFacadeSut.DeleteAsync(reportToDelete.Id);
        
        // Assert
        await using var uow = UnitOfWorkFactory.Create();
        var reportRepository = uow.GetRepository<ReportEntity>();
        var reportRecordsRepository = uow.GetRepository<ReportRecordEntity>();
        Assert.False(await reportRepository.Get().AnyAsync(x => x.Id == reportToDelete.Id));
        Assert.False(await reportRecordsRepository.Get().AnyAsync(x => x.ReportId == reportToDelete.Id));
    }
    
    [Fact]
    public async Task DeleteReportAsync_DoesNotThrowOnNoneExistingReport()
    {
        // Arrange
        await AddMockedEntitiesAsync();
        var nonExistingReportId = Guid.NewGuid();
        
        // Act & Assert
        await ReportFacadeSut.DeleteAsync(nonExistingReportId);
    }

    private async Task AddMockedEntitiesAsync()
    {
        await using var uow = UnitOfWorkFactory.Create();
        var reportRepository = uow.GetRepository<ReportEntity>();
        reportRepository.AddRange(_reports.Select(ReportMapper.ToEntity));
        await uow.CommitAsync();
    }
}