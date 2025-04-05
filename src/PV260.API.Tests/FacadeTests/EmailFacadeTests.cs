using Microsoft.EntityFrameworkCore;
using PV260.API.DAL.Entities;
using PV260.Common.Models;
using PV260.Common.Tests;

namespace PV260.API.Tests.FacadeTests;

public class EmailFacadeTests : FacadeTestBase
{
    private readonly List<EmailRecipientModel> _emails =
    [
        new()
        {
            CreatedAt = new DateTime(2025, 4, 16, 5, 46, 01),
            EmailAddress = "test@test.com"
        },
        new()
        {
            CreatedAt = new DateTime(2025, 4, 16, 5, 51, 23),
            EmailAddress = "test2@test.com"
        }
    ];

    [Fact]
    public async Task GetAllEmailRecipientsAsync_ReturnsAllRecipients()
    {
        // Arrange
        await AddMockedEntitiesAsync();
        
        // Act
        var actualEmails = await EmailFacadeSut.GetAllEmailRecipientsAsync();
        
        // Assert
        Assert.Equal(_emails.Count, actualEmails.Count);
        foreach (var expectedEmail in _emails)
        {
            DeepAssert.Contains(expectedEmail, actualEmails);
        }
    }

    [Fact]
    public async Task AddEmailRecipientAsync_AddsUniqueEmail()
    {
        // Arrange
        var emailRecipientModelToAdd = new EmailRecipientModel()
        {
            CreatedAt = new DateTime(2025, 4, 16, 5, 46, 01),
            EmailAddress = "test@test.com"
        };
        
        // Act
        await EmailFacadeSut.AddEmailRecipientAsync(emailRecipientModelToAdd);
        
        // Assert
        await using var uow = UnitOfWorkFactory.Create();
        var repository = uow.GetRepository<EmailEntity>();
        var actualEmail = await repository.Get()
            .FirstOrDefaultAsync(e => e.EmailAddress == emailRecipientModelToAdd.EmailAddress);
        
        Assert.NotNull(actualEmail);
        DeepAssert.Equal(emailRecipientModelToAdd, EmailMapper.ToDetailModel(actualEmail));
    }

    [Fact]
    public async Task AddEmailRecipientAsync_DoesNotAddDuplicateEmail()
    {
        // Arrange
        await AddMockedEntitiesAsync();
        var emailRecipientModelToAdd = new EmailRecipientModel()
        {
            CreatedAt = new DateTime(2025, 4, 17, 17, 46, 31),
            EmailAddress = "test@test.com"
        };
        
        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await EmailFacadeSut.AddEmailRecipientAsync(emailRecipientModelToAdd));
    }

    [Fact]
    public async Task DeleteEmailRecipientAsync_RemovesExistingEmail()
    {
        // Arrange
        await AddMockedEntitiesAsync();
        var emailToDelete = _emails.First();
        
        // Act
        await EmailFacadeSut.DeleteEmailRecipientAsync(emailToDelete.EmailAddress);
        
        // Assert
        await using var uow = UnitOfWorkFactory.Create();
        var repository = uow.GetRepository<EmailEntity>();
        var actualEmail = await repository.Get()
            .FirstOrDefaultAsync(e => e.EmailAddress == emailToDelete.EmailAddress);
        Assert.Null(actualEmail);
    }

    [Fact]
    public async Task DeleteEmailRecipientAsync_DoesNotThrowOnNonexistentEmail()
    {
        // Arrange
        var emailToDelete = _emails.First();
        
        // Act & Assert
        await EmailFacadeSut.DeleteEmailRecipientAsync(emailToDelete.EmailAddress);
        // Does not throw
    }
    
    [Fact]
    public async Task DeleteAllEmailRecipientsAsync_RemovesAllEmails()
    {
        // Arrange
        await AddMockedEntitiesAsync();
        
        // Act
        await EmailFacadeSut.DeleteAllEmailRecipientsAsync();
        
        // Assert
        await using var uow = UnitOfWorkFactory.Create();
        var repository = uow.GetRepository<EmailEntity>();
        var actualEmails = await repository.Get().ToListAsync();
        
        Assert.Empty(actualEmails);
    }
    
    private async Task AddMockedEntitiesAsync()
    {
        await using var uow = UnitOfWorkFactory.Create();
        var repository = uow.GetRepository<EmailEntity>();
        repository.AddRange(_emails.Select(EmailMapper.ToEntity));
        await uow.CommitAsync();
    }
}