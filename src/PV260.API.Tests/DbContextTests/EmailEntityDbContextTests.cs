using Microsoft.EntityFrameworkCore;
using PV260.API.DAL.Entities;
using PV260.Common.Tests;

namespace PV260.API.Tests.DbContextTests;

public class EmailEntityDbContextTests : DbContextTestBase
{
    [Fact]
    public async Task AddEmailEntity()
    {
        // Arrange
        var entity = new EmailEntity
        {
            EmailAddress = "test@test.com",
            CreatedAt = new DateTime(2025, 4, 13, 2, 12, 22)
        };
        
        // Act
        DbContextSut.Emails.Add(entity);
        await DbContextSut.SaveChangesAsync();
        
        // Assert
        await using var dbx = DbContextFactory.CreateDbContext();
        var actualEntity = await dbx.Emails.SingleAsync(x => x.Id == entity.Id);
        DeepAssert.Equal(entity, actualEntity);
    }
    
    [Fact]
    public async Task AddEmailEntity_DuplicateEmail_Throws()
    {
        // Arrange
        var entity = new EmailEntity
        {
            EmailAddress = "duplicate@test.com",
            CreatedAt = new DateTime(2025, 4, 13, 2, 12, 22)
        };

        DbContextSut.Emails.Add(entity);
        await DbContextSut.SaveChangesAsync();

        var duplicateEntity = new EmailEntity
        {
            EmailAddress = "duplicate@test.com",
            CreatedAt = new DateTime(2025, 4, 14, 3, 15, 30)
        };

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => DbContextSut.Emails.Add(duplicateEntity));
    }
    
    [Fact]
    public void AddEmailEntity_NullEmail_Throws()
    {
        // Arrange
        var entity = new EmailEntity
        {
            EmailAddress = null!,
            CreatedAt = new DateTime(2025, 4, 13, 2, 12, 22)
        };

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => DbContextSut.Emails.Add(entity));
    }
}