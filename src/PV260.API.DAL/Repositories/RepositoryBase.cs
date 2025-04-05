using PV260.API.DAL.Entities;

namespace PV260.API.DAL.Repositories;

public abstract class RepositoryBase<TEntity>(MainDbContext context) : IRepository<TEntity>, IDisposable
    where TEntity : class, IEntity
{

    public ICollection<TEntity> GetAll()
    {
        return context.Set<TEntity>().ToList();
    }

    public TEntity? GetById(Guid id)
    {
        return context.Set<TEntity>().Find(id);
    }

    public void AddOrUpdate(TEntity entity)
    {
        if (Exists(entity.Id))
            context.Set<TEntity>().Update(entity);
        else
            context.Set<TEntity>().Add(entity);

        context.SaveChanges();
    }

    public void Delete(Guid id)
    {
        var entity = GetById(id);
        if (entity is null)
            return;
        
        context.Set<TEntity>().Remove(entity);
        context.SaveChanges();
    }

    public bool Exists(Guid id)
    {
        return context.Set<TEntity>().Any(e => e.Id == id);
    }

    public void Dispose()
    {
        context.Dispose();
        GC.SuppressFinalize(this);
    }
}