using Microsoft.EntityFrameworkCore;
using PV260.API.DAL.Entities;
using PV260.API.DAL.UnitOfWork;
using PV260.API.Tests.Seeds;
using PV260.Common.Tests;

namespace PV260.API.Tests.DbContextTests;

[Collection("EmailEntityDbContextTests")]
public class EmailEntityDbContextTests : DbContextTestBase
{
    [Fact]
    public async Task AddEmailEntity()
    {
        // Arrange
        var entity = new EmailEntity
        {
            EmailAddress = "test123@test.com",
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
        var duplicateEntity = EmailEntitySeeds.SeededEmailEntities.First() with
        {
            Id = Guid.NewGuid(),
            CreatedAt = new DateTime(2025, 4, 13, 2, 12, 22)
        };
        
        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(async () =>
        {
            DbContextSut.Emails.Add(duplicateEntity);
            await DbContextSut.SaveChangesAsync();
        });
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

    [Fact]
    public async Task DeleteEmailEntity_DeletesExistingEntity()
    {
        // Arrange
        var entityToDelete = EmailEntitySeeds.SeededEmailEntities.First();
        
        // Act
        DbContextSut.Emails.Remove(entityToDelete);
        await DbContextSut.SaveChangesAsync();
    }
    
    [Fact]
    public async Task DeleteEmailEntity_NonExistentEntityThrows()
    {
        // Arrange
        var entityToDelete = new EmailEntity
        {
            EmailAddress = "test123@test.com",
            CreatedAt = new DateTime(2025, 4, 13, 2, 12, 22)
        };
        
        // Act & Assert
        DbContextSut.Emails.Remove(entityToDelete);
        await Assert.ThrowsAnyAsync<DbUpdateConcurrencyException>(async () =>
        {
            await DbContextSut.SaveChangesAsync();
        });
    }
}