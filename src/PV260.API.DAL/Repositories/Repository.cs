﻿using Microsoft.EntityFrameworkCore;
using PV260.API.DAL.Entities;

namespace PV260.API.DAL.Repositories;

/// <summary>
/// A generic repository implementation for performing CRUD operations on entities.
/// </summary>
/// <typeparam name="TEntity">The type of the entity that implements <see cref="IEntity"/>.</typeparam>
/// <param name="context">The database context used for accessing the entity set.</param>
public class Repository<TEntity>(DbContext context) : IRepository<TEntity>
    where TEntity : class, IEntity
{

    /// <inheritdoc />
    public IQueryable<TEntity> Get()
    {
        return context.Set<TEntity>()
            .AsNoTracking();
    }

    /// <inheritdoc />
    public async Task<TEntity?> GetByIdAsync(Guid id)
    {
        return await context.Set<TEntity>()
            .AsNoTracking()
            .FirstOrDefaultAsync(entity => entity.Id == id)
            .ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async ValueTask<bool> ExistsAsync(Guid id)
    {
        return await context.Set<TEntity>().AnyAsync(entity => entity.Id == id).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task AddOrUpdateAsync(TEntity entity)
    {
        if (await ExistsAsync(entity.Id).ConfigureAwait(false))
            context.Set<TEntity>().Update(entity);
        else
            context.Set<TEntity>().Add(entity);
    }

    /// <inheritdoc />
    public void AddRange(IEnumerable<TEntity> entities)
    {
        context.Set<TEntity>().AddRange(entities);
    }

    /// <inheritdoc />
    public void Delete(TEntity entity)
    {
        context.Set<TEntity>().Remove(entity);
    }
    
    /// <inheritdoc />
    public void DeleteRange(IEnumerable<TEntity> entities)
    {
        context.Set<TEntity>().RemoveRange(entities);
    }
    
    public async ValueTask DisposeAsync()
    {
        await context.DisposeAsync();
        GC.SuppressFinalize(this);
    }
}