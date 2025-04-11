using Microsoft.EntityFrameworkCore;
using PV260.API.DAL.Entities;
using PV260.Common.Tests;

namespace PV260.API.Tests;

public class DalReportTests : DalTestBase
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
        DbContextSUT.Reports.Add(entity);
        await DbContextSUT.SaveChangesAsync();
        
        // Assert
        await using var dbx = DbContextFactory.CreateDbContext();
        var actualEntity = await dbx.Reports.SingleAsync(x => x.Id == entity.Id);
        DeepAssert.Equal(entity, actualEntity, nameof(ReportEntity.Records));
    }
}