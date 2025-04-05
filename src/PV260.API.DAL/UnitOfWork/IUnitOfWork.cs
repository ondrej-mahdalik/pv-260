using PV260.API.DAL.Entities;
using PV260.API.DAL.Repositories;

namespace PV260.API.DAL.UnitOfWork;

/// <summary>
/// Represents a unit of work that manages repositories and ensures atomic operations.
/// </summary>
public interface IUnitOfWork : IAsyncDisposable
{
    /// <summary>
    /// Retrieves a repository for the specified entity type.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity that implements <see cref="IEntity"/>.</typeparam>
    /// <returns>An instance of <see cref="IRepository{TEntity}"/> for the specified entity type.</returns>
    IRepository<TEntity> GetRepository<TEntity>()
        where TEntity : class, IEntity;

    /// <summary>
    /// Commits all changes made within the unit of work asynchronously.
    /// </summary>
    /// <returns>A task that represents the asynchronous commit operation.</returns>
    Task CommitAsync();
}