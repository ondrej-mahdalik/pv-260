using Microsoft.EntityFrameworkCore;
using PV260.API.DAL.Entities;
using PV260.API.DAL.Repositories;

namespace PV260.API.DAL.UnitOfWork;

/// <summary>
/// Represents a unit of work that manages repositories and ensures atomic operations.
/// </summary>
/// <param name="dbContext">The database context used for managing entities and transactions.</param>
public class UnitOfWork(DbContext dbContext) : IUnitOfWork
{
    private readonly DbContext _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));

    /// <summary>
    /// Retrieves a repository for the specified entity type.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity that implements <see cref="IEntity"/>.</typeparam>
    /// <returns>An instance of <see cref="IRepository{TEntity}"/> for the specified entity type.</returns>
    public IRepository<TEntity> GetRepository<TEntity>() where TEntity : class, IEntity
    {
        return new Repository<TEntity>(_dbContext);
    }

    /// <summary>
    /// Commits all changes made within the unit of work asynchronously.
    /// </summary>
    /// <returns>A task that represents the asynchronous commit operation.</returns>
    public async Task CommitAsync()
    {
        await _dbContext.SaveChangesAsync().ConfigureAwait(false);
    }

    /// <summary>
    /// Disposes the database context asynchronously.
    /// </summary>
    /// <returns>A task that represents the asynchronous dispose operation.</returns>
    public async ValueTask DisposeAsync()
    {
        await _dbContext.DisposeAsync().ConfigureAwait(false);
        GC.SuppressFinalize(this);
    }
}