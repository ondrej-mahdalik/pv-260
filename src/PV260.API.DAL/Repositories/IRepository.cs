using PV260.API.DAL.Entities;

namespace PV260.API.DAL.Repositories;

public interface IRepository<TEntity>
    where TEntity : IEntity
{
    ICollection<TEntity> GetAll();
    TEntity? GetById(Guid id);
    void AddOrUpdate(TEntity entity);
    void Delete(Guid id);
    bool Exists(Guid id);
}