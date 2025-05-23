using Microsoft.EntityFrameworkCore;
using PV260.API.DAL.Entities;
using PV260.API.IntegrationTests.Seeds;
using PV260.Tests.Common;

namespace PV260.API.IntegrationTests.DbContextTests;

[Collection("ReportEntityDbContextTests")]
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
        DeepAssert.Equal(entity, actualEntity);
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
    public async Task AddReportEntity_NullName_Throws()
    {
        // Arrange
        var entity = new ReportEntity
        {
            Name = null!,
            CreatedAt = new DateTime(2025, 4, 11, 4, 13, 22)
        };

        // Act & Assert
        DbContextSut.Reports.Add(entity);
        await Assert.ThrowsAsync<DbUpdateException>(async () => await DbContextSut.SaveChangesAsync());
    }

    [Fact]
    public async Task UpdateReportEntity()
    {
        // Arrange
        var updatedEntity = ReportEntitySeeds.Entity1 with
        {
            Name = "Updated Report"
        };

        // Act
        DbContextSut.Reports.Update(updatedEntity);
        await DbContextSut.SaveChangesAsync();

        // Assert
        await using var dbx = DbContextFactory.CreateDbContext();
        var actualEntity = await dbx.Reports.SingleAsync(x => x.Id == updatedEntity.Id);
        DeepAssert.Equal(updatedEntity, actualEntity, nameof(ReportEntity.Records));
    }

    [Fact]
    public async Task DeleteReportEntity_DeletesExistingEntity()
    {
        // Arrange
        var entityToDelete = ReportEntitySeeds.Entity3;

        // Act
        DbContextSut.Reports.Remove(entityToDelete);
        await DbContextSut.SaveChangesAsync();

        // Assert
        await using var dbx = DbContextFactory.CreateDbContext();
        var actualEntity = await dbx.Reports.SingleOrDefaultAsync(x => x.Id == entityToDelete.Id);
        Assert.Null(actualEntity);
    }

    [Fact]
    public async Task DeleteReportEntity_DeletesExistingEntityWithRecords()
    {
        // Arrange
        var entityToDelete = ReportEntitySeeds.Entity1;

        // Act
        DbContextSut.Reports.Remove(entityToDelete);
        await DbContextSut.SaveChangesAsync();

        // Assert
        await using var dbx = DbContextFactory.CreateDbContext();
        var actualEntity = await dbx.Reports.SingleOrDefaultAsync(x => x.Id == entityToDelete.Id);
        var actualRecords = await dbx.ReportRecords
            .Where(x => x.ReportId == entityToDelete.Id)
            .ToListAsync();
        Assert.Null(actualEntity);
        Assert.Empty(actualRecords);
    }
    
    [Fact]
    public async Task DeleteReportEntity_NonExistentEntityThrows()
    {
        // Arrange
        var entityToDelete = new ReportEntity()
        {
            CreatedAt = new DateTime(2025, 4, 12, 11, 13, 57),
            Name = "Non-existent Report"
        };

        // Act & Assert
        DbContextSut.Reports.Remove(entityToDelete);
        await Assert.ThrowsAsync<DbUpdateConcurrencyException>(async () => { await DbContextSut.SaveChangesAsync(); });
    }
}