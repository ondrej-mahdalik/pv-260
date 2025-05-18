using Microsoft.EntityFrameworkCore;
using PV260.API.BL.Facades;
using PV260.API.BL.Mappers;
using PV260.API.DAL.Entities;
using PV260.API.DAL.UnitOfWork;
using PV260.Common.Models;

namespace PV260.API.Presentation.Facades;

/// <summary>
/// Provides a base implementation for CRUD operations in a facade.
/// </summary>
/// <typeparam name="TListModel">The type of the list model.</typeparam>
/// <typeparam name="TDetailModel">The type of the detail model.</typeparam>
/// <typeparam name="TEntity">The type of the entity.</typeparam>
public abstract class CrudFacadeBase<TListModel, TDetailModel, TEntity>(
    IMapper<TEntity, TListModel, TDetailModel> mapper,
    IUnitOfWorkFactory unitOfWorkFactory)
    : ICrudFacade<TListModel, TDetailModel, TEntity>
    where TListModel : class, IDataModel
    where TDetailModel : class, IDataModel
    where TEntity : class, IEntity
{
    /// <summary>
    /// The mapper used for converting between entities and models.
    /// </summary>
    protected readonly IMapper<TEntity, TListModel, TDetailModel> Mapper = mapper;

    /// <summary>
    /// The factory for creating unit of work instances.
    /// </summary>
    protected readonly IUnitOfWorkFactory UnitOfWorkFactory = unitOfWorkFactory;

    /// <inheritdoc />
    public virtual async Task<IList<TListModel>> GetAsync()
    {
        await using var uow = UnitOfWorkFactory.Create();
        var repository = uow.GetRepository<TEntity>();
        var entities = await repository.Get().ToListAsync();

        return Mapper.ToListModel(entities).ToList();
    }

    /// <inheritdoc />
    public virtual async Task<TDetailModel?> GetAsync(Guid id)
    {
        await using var uow = UnitOfWorkFactory.Create();
        var entity = await uow.GetRepository<TEntity>()
            .GetByIdAsync(id);

        return entity is null
            ? null
            : Mapper.ToDetailModel(entity);
    }

    /// <inheritdoc />
    public virtual async Task SaveAsync(TDetailModel model)
    {
        await using var uow = UnitOfWorkFactory.Create();
        var repository = uow.GetRepository<TEntity>();

        var entity = Mapper.ToEntity(model);
        await repository.AddOrUpdateAsync(entity);
        await uow.CommitAsync().ConfigureAwait(false);
    }

    /// <inheritdoc />
    public virtual async Task SaveAsync(IEnumerable<TDetailModel> models)
    {
        await using var uow = UnitOfWorkFactory.Create();
        var repository = uow.GetRepository<TEntity>();

        var entities = Mapper.ToEntity(models);
        foreach (var entity in entities)
        {
            await repository.AddOrUpdateAsync(entity);
        }

        await uow.CommitAsync().ConfigureAwait(false);
    }

    /// <inheritdoc />
    public virtual async Task DeleteAsync(Guid id)
    {
        await using var uow = UnitOfWorkFactory.Create();
        var repository = uow.GetRepository<TEntity>();
        
        var entity = await repository.GetByIdAsync(id);
        if (entity is null)
            return;

        try
        {
            repository.Delete(entity);
            await uow.CommitAsync().ConfigureAwait(false);
        }
        catch (DbUpdateException ex)
        {
            throw new InvalidOperationException($"Unable to delete '{typeof(TEntity)}' entity with Id '{id}'", ex);
        }
    }
}