using Microsoft.EntityFrameworkCore;
using Moq;
using PV260.API.DAL.Entities;
using PV260.API.Tests.Seeds;
using PV260.Common.Models;
using PV260.Tests.Common;

namespace PV260.API.Tests.FacadeTests;

[Collection("ReportFacadeTests")]
public class ReportFacadeTests : FacadeTestBase
{
    [Fact]
    public async Task GetAllReportsAsync_ReturnsAllReports()
    {
        // Arrange
        var paginationParameters = new PaginationParameters
        {
            PageNumber = 0,
            PageSize = 10,
        };

        // Act
        var paginatedResponse = await ReportFacadeSut.GetAsync(paginationParameters);

        // Assert
        Assert.Equal(3, paginatedResponse.TotalCount);
        DeepAssert.Contains(ReportMapper.ToListModel(ReportEntitySeeds.Entity1), paginatedResponse.Items);
        DeepAssert.Contains(ReportMapper.ToListModel(ReportEntitySeeds.Entity2), paginatedResponse.Items);
        DeepAssert.Contains(ReportMapper.ToListModel(ReportEntitySeeds.Entity3), paginatedResponse.Items);
    }

    [Fact]
    public async Task GetReportByIdAsync_ReturnsCorrectReport()
    {
        // Arrange
        var reportToGet = ReportMapper.ToDetailModel(ReportEntitySeeds.Entity1);

        // Act
        var actualReport = await ReportFacadeSut.GetAsync(reportToGet.Id);

        // Assert
        Assert.NotNull(actualReport);
        Assert.NotEmpty(actualReport.Records);
        DeepAssert.Equal(reportToGet, actualReport, nameof(ReportDetailModel.Records));
    }

    [Fact]
    public async Task GetReportByIdAsync_ReturnsNull_WhenReportDoesNotExist()
    {
        // Arrange
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
        var reportToUpdate = ReportMapper.ToDetailModel(ReportEntitySeeds.Entity1) with
        {
            Name = "Updated Report"
        };
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
    public async Task DeleteReportAsync_DeletesExistingReportIncludingReports()
    {
        // Arrange
        var reportToDelete = ReportMapper.ToDetailModel(ReportEntitySeeds.Entity1);

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
        var nonExistingReportId = Guid.NewGuid();

        // Act & Assert
        await ReportFacadeSut.DeleteAsync(nonExistingReportId);
    }

    [Fact]
    public async Task GenerateReportAsync_CreatesAndSavesNewReport()
    {

        // Act
        var report = await ReportFacadeSut.GenerateReportAsync();

        // Assert
        Assert.NotNull(report);
        Assert.True(report.Records.Count > 0, "Report should contain records.");
        Assert.All(report.Records, r => Assert.False(string.IsNullOrEmpty(r.CompanyName), "CompanyName should not be null or empty."));
    }

    [Fact]
    public async Task DeleteOldReportsAsync_DeletesReportsOlderThanRetentionPeriod()
    {
        // Arrange
        var oldReport = new ReportEntity
        {
            Id = Guid.NewGuid(),
            Name = "Old Report",
            CreatedAt = DateTime.UtcNow.AddDays(-40)
        };
        await using var uow = UnitOfWorkFactory.Create();
        var reportRepository = uow.GetRepository<ReportEntity>();
        await reportRepository.AddOrUpdateAsync(oldReport);
        await uow.CommitAsync();

        // Act
        var deletedCount = await ReportFacadeSut.DeleteOldReportsAsync();

        // Assert
        Assert.Equal(1, deletedCount);
        Assert.False(await reportRepository.Get().AnyAsync(r => r.Id == oldReport.Id));
    }

    [Fact]
    public async Task SendReportViaEmailAsync_CallsEmailServiceWithCorrectParameters()
    {
        // Arrange
        EmailServiceMock.Setup(action => action.SendEmailAsync(It.IsAny<IList<string>>(), It.IsAny<string>(), It.IsAny<string>()))
            .Returns(Task.CompletedTask);
        var reportId = ReportEntitySeeds.Entity1.Id;
        
        // Act
        await ReportFacadeSut.SendReportViaEmailAsync(reportId);
        
        // Assert
        List<string> recipientAddresses =
        [
            EmailEntitySeeds.Entity1.EmailAddress,
            EmailEntitySeeds.Entity2.EmailAddress
        ];
        EmailServiceMock.Verify(
            action => action.SendEmailAsync(recipientAddresses, It.IsAny<string>(), It.IsAny<string>()), Times.Once);
    }
    
    [Fact]
    public async Task SendReportViaEmailAsync_ThrowsException_WhenReportNotFound()
    {
        // Arrange
        var nonExistingReportId = Guid.NewGuid();
        
        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await ReportFacadeSut.SendReportViaEmailAsync(nonExistingReportId));
    }
}