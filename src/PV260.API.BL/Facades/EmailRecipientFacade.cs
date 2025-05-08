using Microsoft.EntityFrameworkCore;
using PV260.API.BL.Mappers;
using PV260.API.DAL.Entities;
using PV260.API.DAL.Repositories;
using PV260.API.DAL.UnitOfWork;
using PV260.Common.Models;

namespace PV260.API.BL.Facades;

/// <inheritdoc />
public class EmailRecipientFacade(
    IMapper<EmailRecipientEntity, EmailRecipientModel, EmailRecipientModel> emailMapper,
    IUnitOfWorkFactory unitOfWorkFactory) : IEmailFacade
{
    /// <inheritdoc />
    public async Task<IList<EmailRecipientModel>> GetAllEmailRecipientsAsync()
    {
        await using var uow = unitOfWorkFactory.Create();
        var emails = await uow.GetRepository<EmailRecipientEntity>().Get().ToListAsync();

        return emails.Select(emailMapper.ToListModel).ToList();
    }

    /// <inheritdoc />
    public async Task<PaginatedResponse<EmailRecipientModel>> GetAllEmailRecipientsAsync(PaginationParameters paginationParameters)
    {
        await using var uow = unitOfWorkFactory.Create();

        var repository = uow.GetRepository<EmailRecipientEntity>();

        var emails = await repository
            .Get()
            .Skip((paginationParameters.PageNumber - 1) * paginationParameters.PageSize)
            .Take(paginationParameters.PageSize)
            .ToListAsync();

        var totalNumberOfEntities = await repository.Get().CountAsync();

        return new PaginatedResponse<EmailRecipientModel>
        {
            Items = emailMapper.ToListModel(emails),
            PageSize = paginationParameters.PageSize,
            PageNumber = paginationParameters.PageNumber,
            TotalCount = totalNumberOfEntities
        };
    }

    /// <inheritdoc />
    /// <exception cref="InvalidOperationException">Thrown when the operation fails to insert new entry.
    /// Can be caused by the email address not being unique.</exception>
    public async Task AddEmailRecipientAsync(EmailRecipientModel emailRecipientModel)
    {
        await using var uow = unitOfWorkFactory.Create();
        var repository = uow.GetRepository<EmailRecipientEntity>();
        
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
        var repository = uow.GetRepository<EmailRecipientEntity>();
        
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
        var repository = uow.GetRepository<EmailRecipientEntity>();
        var entities = await repository.Get().ToListAsync();
        
        foreach (var entity in entities)
            repository.Delete(entity);
        
        await uow.CommitAsync();
    }
}