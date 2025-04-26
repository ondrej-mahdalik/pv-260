using Microsoft.EntityFrameworkCore;
using PV260.API.DAL.Entities;
using PV260.API.Tests.Seeds;
using PV260.Common.Models;
using PV260.Tests.Common;

namespace PV260.API.Tests.FacadeTests;

[Collection("EmailFacadeTests")]
public class EmailRecipientFacadeTests : FacadeTestBase
{
    [Fact]
    public async Task GetAllEmailRecipientsAsync_ReturnsAllRecipients()
    {
        // Arrange
        // Act
        var actualEmails = await EmailRecipientFacadeSut.GetAllEmailRecipientsAsync();

        // Assert
        Assert.Equal(2, actualEmails.Count);
        DeepAssert.Contains(EmailRecipientMapper.ToDetailModel(EmailEntitySeeds.Entity1), actualEmails);
        DeepAssert.Contains(EmailRecipientMapper.ToDetailModel(EmailEntitySeeds.Entity2), actualEmails);
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
        await EmailRecipientFacadeSut.AddEmailRecipientAsync(emailRecipientModelToAdd);

        // Assert
        await using var uow = UnitOfWorkFactory.Create();
        var repository = uow.GetRepository<EmailRecipientEntity>();
        var actualEmail = await repository.Get()
            .FirstOrDefaultAsync(e => e.EmailAddress == emailRecipientModelToAdd.EmailAddress);

        Assert.NotNull(actualEmail);
        DeepAssert.Equal(emailRecipientModelToAdd, EmailRecipientMapper.ToDetailModel(actualEmail));
    }

    [Fact]
    public async Task AddEmailRecipientAsync_DoesNotAddDuplicateEmail()
    {
        // Arrange
        var emailRecipientModelToAdd = new EmailRecipientModel
        {
            CreatedAt = new DateTime(2025, 4, 17, 17, 46, 31),
            EmailAddress = EmailEntitySeeds.Entity1.EmailAddress
        };

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await EmailRecipientFacadeSut.AddEmailRecipientAsync(emailRecipientModelToAdd));
    }

    [Fact]
    public async Task DeleteEmailRecipientAsync_RemovesExistingEmail()
    {
        // Arrange
        var emailRecipientToDelete = EmailRecipientMapper.ToDetailModel(EmailEntitySeeds.Entity1);

        // Act
        await EmailRecipientFacadeSut.DeleteEmailRecipientAsync(emailRecipientToDelete.EmailAddress);

        // Assert
        await using var uow = UnitOfWorkFactory.Create();
        var repository = uow.GetRepository<EmailRecipientEntity>();
        var actualEmail = await repository.Get()
            .FirstOrDefaultAsync(e => e.EmailAddress == emailRecipientToDelete.EmailAddress);
        Assert.Null(actualEmail);
    }

    [Fact]
    public async Task DeleteEmailRecipientAsync_DoesNotThrowOnNonexistentEmail()
    {
        // Arrange
        var emailToDelete = "test123@test.com";

        // Act & Assert
        await EmailRecipientFacadeSut.DeleteEmailRecipientAsync(emailToDelete);
        // Does not throw
    }

    [Fact]
    public async Task DeleteAllEmailRecipientsAsync_RemovesAllEmails()
    {
        // Arrange
        // Act
        await EmailRecipientFacadeSut.DeleteAllEmailRecipientsAsync();

        // Assert
        await using var uow = UnitOfWorkFactory.Create();
        var repository = uow.GetRepository<EmailRecipientEntity>();
        var actualEmails = await repository.Get().ToListAsync();

        Assert.Empty(actualEmails);
    }
}