using Microsoft.EntityFrameworkCore;
using PV260.API.DAL.Entities;
using PV260.Common.Tests;

namespace PV260.API.Tests.DbContextTests;

public class ReportEntityDbContextTests : DbContextTestBase
{
    
    [Fact]
    public async Task AddReportEntity_WithoutRecords()
    {
        // Arrange
        var entity = new ReportEntity
        {
            Name = "Test Report",
            CreatedAt = new DateTime(2025, 4, 11, 4, 13, 22)
        };
        
        // Act
        DbContextSut.Reports.Add(entity);
        await DbContextSut.SaveChangesAsync();
        
        // Assert
        await using var dbx = DbContextFactory.CreateDbContext();
        var actualEntity = await dbx.Reports.SingleAsync(x => x.Id == entity.Id);
        DeepAssert.Equal(entity, actualEntity, nameof(ReportEntity.Records));
    }
    
    [Fact]
    public async Task AddReportEntity_WithRecords()
    {
        // Arrange
        var entity = new ReportEntity
        {
            Name = "Test Report",
            CreatedAt = new DateTime(2025, 4, 11, 4, 13, 22),
            Records = new List<ReportRecordEntity>
            {
                new()
                {
                    CompanyName = "Test Company",
                    Ticker = "Test Ticker",
                    Weight = 2.5,
                    NumberOfShares = 200,
                    SharesChangePercentage = 0.2
                }
            }
        };
        
        // Act
        DbContextSut.Reports.Add(entity);
        await DbContextSut.SaveChangesAsync();
        
        // Assert
        await using var dbx = DbContextFactory.CreateDbContext();
        var actualEntity = await dbx.Reports
            .Include(x => x.Records)
            .SingleAsync(x => x.Id == entity.Id);
        DeepAssert.Equal(entity, actualEntity);
    }
    
    [Fact]
    public async Task UpdateReportEntity()
    {
        // Arrange
        var entity = new ReportEntity
        {
            Name = "Test Report",
            CreatedAt = new DateTime(2025, 4, 11, 4, 13, 22)
        };
        
        DbContextSut.Reports.Add(entity);
        await DbContextSut.SaveChangesAsync();
        
        // Act
        entity.Name = "Updated Report";
        await DbContextSut.SaveChangesAsync();
        
        // Assert
        await using var dbx = DbContextFactory.CreateDbContext();
        var actualEntity = await dbx.Reports.SingleAsync(x => x.Id == entity.Id);
        DeepAssert.Equal(entity, actualEntity);
    }

    [Fact]
    public async Task UpdateReportEntity_AddRecord()
    {
        // Arrange
        var entity = new ReportEntity
        {
            Name = "Test Report",
            CreatedAt = new DateTime(2025, 4, 11, 4, 13, 22)
        };
        DbContextSut.Reports.Add(entity);
        await DbContextSut.SaveChangesAsync();
        
        // Act
        entity.Records.Add(new ReportRecordEntity
        {
            CompanyName = "Test Company",
            Ticker = "Test Ticker",
            Weight = 2.5,
            NumberOfShares = 200,
            SharesChangePercentage = 0.2
        });
        await DbContextSut.SaveChangesAsync();
        
        // Assert
        await using var dbx = DbContextFactory.CreateDbContext();
        var actualEntity = await dbx.Reports
            .Include(x => x.Records)
            .SingleAsync(x => x.Id == entity.Id);
        DeepAssert.Equal(entity.Records, actualEntity.Records);
    }
    
    [Fact]
    public async Task UpdateReportEntity_UpdateRecord()
    {
        // Arrange
        var entity = new ReportEntity
        {
            Name = "Test Report",
            CreatedAt = new DateTime(2025, 4, 11, 4, 13, 22),
            Records = new List<ReportRecordEntity>
            {
                new()
                {
                    CompanyName = "Test Company",
                    Ticker = "Test Ticker",
                    Weight = 2.5,
                    NumberOfShares = 200,
                    SharesChangePercentage = 0.2
                }
            }
        };
        
        DbContextSut.Reports.Add(entity);
        await DbContextSut.SaveChangesAsync();
        
        // Act
        var recordToUpdate = entity.Records.First();
        recordToUpdate.CompanyName = "Updated Company";
        await DbContextSut.SaveChangesAsync();
        
        // Assert
        await using var dbx = DbContextFactory.CreateDbContext();
        var actualEntity = await dbx.Reports
            .Include(x => x.Records)
            .SingleAsync(x => x.Id == entity.Id);
        DeepAssert.Equal(entity.Records, actualEntity.Records);
    }

    [Fact]
    public async Task UpdateReportEntity_RemoveRecord()
    {
        // Arrange
        var entity = new ReportEntity
        {
            Name = "Test Report",
            CreatedAt = new DateTime(2025, 4, 11, 4, 13, 22),
            Records = new List<ReportRecordEntity>
            {
                new()
                {
                    CompanyName = "Test Company",
                    Ticker = "Test Ticker",
                    Weight = 2.5,
                    NumberOfShares = 200,
                    SharesChangePercentage = 0.2
                }
            }
        };
        
        DbContextSut.Reports.Add(entity);
        await DbContextSut.SaveChangesAsync();
        
        // Act
        entity.Records.Clear();
        await DbContextSut.SaveChangesAsync();
        
        // Assert
        await using var dbx = DbContextFactory.CreateDbContext();
        var actualEntity = await dbx.Reports
            .Include(x => x.Records)
            .SingleAsync(x => x.Id == entity.Id);
        var actualRecords = await dbx.ReportRecords
            .Where(x => x.ReportId == entity.Id)
            .ToListAsync();
        DeepAssert.Equal(entity, actualEntity);
        Assert.Empty(actualRecords);
    }
    
    [Fact]
    public async Task DeleteReportEntity()
    {
        // Arrange
        var entity = new ReportEntity
        {
            Name = "Test Report",
            CreatedAt = new DateTime(2025, 4, 11, 4, 13, 22)
        };
        
        DbContextSut.Reports.Add(entity);
        await DbContextSut.SaveChangesAsync();
        
        // Act
        DbContextSut.Reports.Remove(entity);
        await DbContextSut.SaveChangesAsync();
        
        // Assert
        await using var dbx = DbContextFactory.CreateDbContext();
        var actualEntity = await dbx.Reports.SingleOrDefaultAsync(x => x.Id == entity.Id);
        Assert.Null(actualEntity);
    }
    
    [Fact]
    public async Task DeleteReportEntity_WithRecords()
    {
        // Arrange
        var entity = new ReportEntity
        {
            Name = "Test Report",
            CreatedAt = new DateTime(2025, 4, 11, 4, 13, 22),
            Records = new List<ReportRecordEntity>
            {
                new()
                {
                    CompanyName = "Test Company",
                    Ticker = "Test Ticker",
                    Weight = 2.5,
                    NumberOfShares = 200,
                    SharesChangePercentage = 0.2
                }
            }
        };
        
        DbContextSut.Reports.Add(entity);
        await DbContextSut.SaveChangesAsync();
        
        // Act
        DbContextSut.Reports.Remove(entity);
        await DbContextSut.SaveChangesAsync();
        
        // Assert
        await using var dbx = DbContextFactory.CreateDbContext();
        var actualEntity = await dbx.Reports.SingleOrDefaultAsync(x => x.Id == entity.Id);
        var actualRecords = await dbx.ReportRecords
            .Where(x => x.ReportId == entity.Id)
            .ToListAsync();
        Assert.Null(actualEntity);
        Assert.Empty(actualRecords);
    }

    [Fact]
    public async Task AddReportEntity_NullName_Throws()
    {
        // Arrange
        var entity = new ReportEntity
        {
            Name = null!,
            CreatedAt = new DateTime(2025, 4, 11, 4, 13, 22)
        };
        
        // Act & Assert
        await Assert.ThrowsAsync<DbUpdateException>(async () =>
        {
            DbContextSut.Reports.Add(entity);
            await DbContextSut.SaveChangesAsync();
        });
    }
}