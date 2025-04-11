using PV260.API.DAL.Entities;

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
    /// <returns>A task that represents the asynchronous operation. The task result contains the entity if found, otherwise null.</returns>
    Task<TEntity?> GetByIdAsync(Guid id);

    /// <summary>
    /// Checks if an entity with the specified unique identifier exists asynchronously.
    /// </summary>
    /// <param name="id">The unique identifier of the entity.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains true if the entity exists, otherwise false.</returns>
    ValueTask<bool> ExistsAsync(Guid id);

    /// <summary>
    /// Adds a new entity or updates an existing entity in the repository.
    /// </summary>
    /// <param name="entity">The entity to add or update.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task AddOrUpdateAsync(TEntity entity);

    /// <summary>
    /// Deletes an entity from the repository asynchronously.
    /// </summary>
    /// <param name="entity">The entity to delete.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    void Delete(TEntity entity);
}