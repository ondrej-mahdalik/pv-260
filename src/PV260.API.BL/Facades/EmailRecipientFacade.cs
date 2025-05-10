using Microsoft.EntityFrameworkCore;
using PV260.API.BL.Mappers;
using PV260.API.DAL.Entities;
using PV260.API.DAL.UnitOfWork;
using PV260.Common.Models;

namespace PV260.API.BL.Facades;

/// <inheritdoc />
public class EmailRecipientFacade(
    IMapper<EmailRecipientEntity, EmailRecipientModel, EmailRecipientModel> emailMapper,
    IUnitOfWorkFactory unitOfWorkFactory) : IEmailFacade
{
    public async Task<PaginatedResponse<EmailRecipientModel>> GetAllEmailRecipientsAsync(
        PaginationCursor paginationCursor)
    {
        await using var uow = unitOfWorkFactory.Create();
        var repository = uow.GetRepository<EmailRecipientEntity>();

        var query = repository
            .Get()
            .OrderByDescending(report => report.CreatedAt)
            .ThenBy(report => report.Id);

        if (paginationCursor.LastCreatedAt is not null && paginationCursor.LastId is not null)
        {
            query = (IOrderedQueryable<EmailRecipientEntity>)query.Where(report =>
                report.CreatedAt < paginationCursor.LastCreatedAt ||
                (report.CreatedAt == paginationCursor.LastCreatedAt && report.Id > paginationCursor.LastId));
        }

        var emailRecipientEntities = await query
            .Take(paginationCursor.PageSize)
            .ToListAsync();

        return new PaginatedResponse<EmailRecipientModel>
        {
            Items = emailMapper.ToListModel(emailRecipientEntities),
            PageSize = paginationCursor.PageSize,
            TotalCount = await repository.Get().CountAsync(),
            NextCursor = emailRecipientEntities.LastOrDefault() is
                         {
                         } last &&
                         last.CreatedAt != paginationCursor.LastCreatedAt &&
                         last.Id != paginationCursor.LastId
                ? new PaginationCursor
                {
                    LastCreatedAt = last.CreatedAt,
                    LastId = last.Id,
                    PageSize = paginationCursor.PageSize
                }
                : null,
        };
    }

    /// <inheritdoc />
    public async Task<IList<EmailRecipientModel>> GetAllEmailRecipientsAsync()
    {
        await using var uow = unitOfWorkFactory.Create();
        var emails = await uow.GetRepository<EmailRecipientEntity>().Get().ToListAsync();
        
        return emails.Select(emailMapper.ToListModel).ToList();
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