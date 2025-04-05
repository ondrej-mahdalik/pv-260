using PV260.API.DAL.Entities;
using PV260.API.DAL.UnitOfWork;

namespace PV260.API.DAL.Repositories;

/// <summary>
/// Generic repository interface for performing CRUD operations on entities.
/// </summary>
/// <typeparam name="TEntity">The type of the entity that implements <see cref="IEntity"/>.</typeparam>
public interface IRepository<TEntity> : IAsyncDisposable
    where TEntity : IEntity
{
    /// <summary>
    /// Retrieves a queryable collection of all entities.
    /// </summary>
    /// <returns>An <see cref="IQueryable{T}"/> representing the collection of entities.</returns>
    IQueryable<TEntity> Get();

    /// <summary>
    /// Retrieves an entity by its unique identifier asynchronously.
    /// </summary>
    /// <param name="id">The unique identifier of the entity.</param>
    /// <returns>The entity if found, otherwise null.</returns>
    Task<TEntity?> GetByIdAsync(Guid id);

    /// <summary>
    /// Checks if an entity with the specified unique identifier exists asynchronously.
    /// </summary>
    /// <param name="id">The unique identifier of the entity.</param>
    /// <returns>True if the entity exists, otherwise false.</returns>
    ValueTask<bool> ExistsAsync(Guid id);

    /// <summary>
    /// Adds a new entity or updates an existing entity in the repository.
    /// Changes are not saved to the database until <see cref="IUnitOfWork.CommitAsync"/> is called.
    /// </summary>
    /// <param name="entity">The entity to add or update.</param>
    Task AddOrUpdateAsync(TEntity entity);

    /// <summary>
    /// Adds a range of entities in the repository. If any entity already exists, operation will fail.
    /// Changes are not saved to the database until <see cref="IUnitOfWork.CommitAsync"/> is called.
    /// </summary>
    /// <param name="entities">The entities to add.</param>
    void AddRange(IEnumerable<TEntity> entities);

    /// <summary>
    /// Deletes an entity from the repository.
    /// Changes are not saved to the database until <see cref="IUnitOfWork.CommitAsync"/> is called.
    /// </summary>
    /// <param name="entity">The entity to delete.</param>
    void Delete(TEntity entity);

    /// <summary>
    /// Deletes a range of entities from the repository.
    /// Changes are not saved to the database until <see cref="IUnitOfWork.CommitAsync"/> is called.
    /// </summary>
    /// <param name="entities">The entities to delete.</param>
    void DeleteRange(IEnumerable<TEntity> entities);
}