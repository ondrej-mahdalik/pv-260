using Microsoft.EntityFrameworkCore;
using PV260.API.DAL.Entities;
using PV260.API.Tests.Seeds;
using PV260.Common.Models;
using PV260.Common.Tests;

namespace PV260.API.Tests.FacadeTests;

[Collection("EmailFacadeTests")]
public class EmailFacadeTests : FacadeTestBase
{
    
    
    [Fact]
    public async Task GetAllEmailRecipientsAsync_ReturnsAllRecipients()
    {
        // Arrange
        // Act
        var actualEmails = await EmailFacadeSut.GetAllEmailRecipientsAsync();

        // Assert
        Assert.Equal(EmailEntitySeeds.SeededEmailEntities.Count, actualEmails.Count);
        foreach (var expectedEmail in EmailRecipientModelSeeds)
            DeepAssert.Contains(expectedEmail, actualEmails);
    }

    [Fact]
    public async Task AddEmailRecipientAsync_AddsUniqueEmail()
    {
        // Arrange
        var emailRecipientModelToAdd = new EmailRecipientModel
        {
            CreatedAt = new DateTime(2025, 4, 16, 5, 46, 01),
            EmailAddress = "test123@test.com"
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
        var emailRecipientModelToAdd = new EmailRecipientModel
        {
            CreatedAt = new DateTime(2025, 4, 17, 17, 46, 31),
            EmailAddress = EmailRecipientModelSeeds.First().EmailAddress
        };

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await EmailFacadeSut.AddEmailRecipientAsync(emailRecipientModelToAdd));
    }

    [Fact]
    public async Task DeleteEmailRecipientAsync_RemovesExistingEmail()
    {
        // Arrange
        var emailRecipientToDelete = EmailRecipientModelSeeds.First();

        // Act
        await EmailFacadeSut.DeleteEmailRecipientAsync(emailRecipientToDelete.EmailAddress);

        // Assert
        await using var uow = UnitOfWorkFactory.Create();
        var repository = uow.GetRepository<EmailEntity>();
        var actualEmail = await repository.Get()
            .FirstOrDefaultAsync(e => e.EmailAddress == emailRecipientToDelete.EmailAddress);
        Assert.Null(actualEmail);
    }

    [Fact]
    public async Task DeleteEmailRecipientAsync_DoesNotThrowOnNonexistentEmail()
    {
        // Arrange
        var emailToDelete = EmailRecipientModelSeeds.First();

        // Act & Assert
        await EmailFacadeSut.DeleteEmailRecipientAsync(emailToDelete.EmailAddress);
        // Does not throw
    }

    [Fact]
    public async Task DeleteAllEmailRecipientsAsync_RemovesAllEmails()
    {
        // Arrange
        // Act
        await EmailFacadeSut.DeleteAllEmailRecipientsAsync();

        // Assert
        await using var uow = UnitOfWorkFactory.Create();
        var repository = uow.GetRepository<EmailEntity>();
        var actualEmails = await repository.Get().ToListAsync();

        Assert.Empty(actualEmails);
    }
}