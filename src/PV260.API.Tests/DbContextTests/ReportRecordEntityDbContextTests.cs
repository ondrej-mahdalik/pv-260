using Microsoft.EntityFrameworkCore;
using PV260.API.DAL.Entities;
using PV260.API.Tests.Seeds;
using PV260.Tests.Common;

namespace PV260.API.Tests.DbContextTests;

[Collection("ReportRecordEntityDbContextTests")]
public class ReportRecordEntityDbContextTests : DbContextTestBase
{
    [Fact]
    public async Task AddReportRecordEntity_AddsRecordsWithParentReport()
    {
        // Arrange
        var entity = new ReportRecordEntity
        {
            CompanyName = "New Company",
            NumberOfShares = 400,
            SharesChangePercentage = 0.4,
            Ticker = "New Ticker",
            Weight = 4.0,
            ReportId = ReportEntitySeeds.Entity2.Id
        };
        
        // Act
        DbContextSut.ReportRecords.Add(entity);
        await DbContextSut.SaveChangesAsync();
        
        // Assert
        await using var dbx = DbContextFactory.CreateDbContext();
        var actualEntity = await dbx.ReportRecords.SingleAsync(x => x.Id == entity.Id);
        DeepAssert.Equal(entity, actualEntity, nameof(ReportRecordEntity.Report));
    }

    [Fact]
    public async Task AddReportRecordEntity_ThrowsOnEntityWithNoParentReport()
    {
        // Arrange
        var entity = new ReportRecordEntity
        {
            CompanyName = "New Company",
            NumberOfShares = 400,
            SharesChangePercentage = 0.4,
            Ticker = "New Ticker",
            Weight = 4.0,
            ReportId = null
        };
        
        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(async () =>
        {
            DbContextSut.ReportRecords.Add(entity);
            await DbContextSut.SaveChangesAsync();
        });
    }
    
    [Fact]
    public async Task AddReportRecordEntity_ThrowsOnEntityWithInvalidParentReport()
    {
        // Arrange
        var entity = new ReportRecordEntity
        {
            CompanyName = "New Company",
            NumberOfShares = 400,
            SharesChangePercentage = 0.4,
            Ticker = "New Ticker",
            Weight = 4.0,
            ReportId = Guid.NewGuid()
        };
        
        // Act & Assert
        await Assert.ThrowsAsync<DbUpdateException>(async () =>
        {
            DbContextSut.ReportRecords.Add(entity);
            await DbContextSut.SaveChangesAsync();
        });
    }
    
    [Fact]
    public async Task UpdateReportRecordEntity_UpdatesExistingRecord()
    {
        // Arrange
        var entityToUpdate = ReportRecordEntitySeeds.Entity1 with
        {
            CompanyName = "Updated Company"
        };
        
        // Act
        DbContextSut.ReportRecords.Update(entityToUpdate);
        await DbContextSut.SaveChangesAsync();
        
        // Assert
        await using var dbx = DbContextFactory.CreateDbContext();
        var actualEntity = await dbx.ReportRecords.SingleAsync(x => x.Id == entityToUpdate.Id);
        DeepAssert.Equal(entityToUpdate, actualEntity, nameof(ReportRecordEntity.Report));
    }
    
    [Fact]
    public async Task DeleteReportRecordEntity_DeletesExistingRecord()
    {
        // Arrange
        var entityToDelete = ReportRecordEntitySeeds.Entity1;
        
        // Act
        DbContextSut.ReportRecords.Remove(entityToDelete);
        await DbContextSut.SaveChangesAsync();
        
        // Assert
        await using var dbx = DbContextFactory.CreateDbContext();
        Assert.False(await dbx.ReportRecords.AnyAsync(x => x.Id == entityToDelete.Id));
    }
}