using Microsoft.EntityFrameworkCore;
using PV260.API.BL.Mappers;
using PV260.API.DAL.Entities;
using PV260.API.DAL.UnitOfWork;
using PV260.Common.Models;

namespace PV260.API.BL.Facades;

/// <inheritdoc />
public class EmailFacade(
    IMapper<EmailEntity, EmailRecipientModel, EmailRecipientModel> emailMapper,
    IUnitOfWorkFactory unitOfWorkFactory) : IEmailFacade
{
    /// <inheritdoc />
    public async Task<IList<EmailRecipientModel>> GetAllEmailRecipientsAsync()
    {
        await using var uow = unitOfWorkFactory.Create();
        var emails = await uow.GetRepository<EmailEntity>().Get().ToListAsync();
        
        return emails.Select(emailMapper.ToListModel).ToList();
    }

    /// <inheritdoc />
    /// <exception cref="InvalidOperationException">Thrown when the operation fails to insert new entry.
    /// Can be caused by the email address not being unique.</exception>
    public async Task AddEmailRecipientAsync(EmailRecipientModel emailRecipientModel)
    {
        await using var uow = unitOfWorkFactory.Create();
        var repository = uow.GetRepository<EmailEntity>();
        
        var entity = emailMapper.ToEntity(emailRecipientModel);

        try
        {
            await repository.AddOrUpdateAsync(entity);
            await uow.CommitAsync();
        }
        catch (DbUpdateException)
        {
            throw new InvalidOperationException("Failed to add email recipient. Make sure the email address is unique.");
        }
    }

    /// <inheritdoc />
    public async Task DeleteEmailRecipientAsync(string emailAddress)
    {
        await using var uow = unitOfWorkFactory.Create();
        var repository = uow.GetRepository<EmailEntity>();
        
        // Check if the email exists
        var entity = await repository.Get().FirstOrDefaultAsync(e => e.EmailAddress == emailAddress);
        if (entity is null)
            return;
        
        repository.Delete(entity);
        await uow.CommitAsync();
    }

    /// <inheritdoc />
    public async Task DeleteAllEmailRecipientsAsync()
    {
        await using var uow = unitOfWorkFactory.Create();
        var repository = uow.GetRepository<EmailEntity>();
        var entities = await repository.Get().ToListAsync();
        
        foreach (var entity in entities)
            repository.Delete(entity);
        
        await uow.CommitAsync();
    }
}