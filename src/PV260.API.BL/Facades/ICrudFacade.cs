using PV260.API.DAL.Entities;
using PV260.Common.Models;

namespace PV260.API.BL.Facades;

/// <summary>
/// Defines a contract for a facade that provides CRUD operations for a specific entity type.
/// </summary>
/// <typeparam name="TListModel">The type of the list model.</typeparam>
/// <typeparam name="TDetailModel">The type of the detail model.</typeparam>
/// <typeparam name="TEntity">The type of the entity.</typeparam>
public interface ICrudFacade<TListModel, TDetailModel, TEntity>
    where TListModel : class, IDataModel
    where TDetailModel : class, IDataModel
    where TEntity : class, IEntity
{
    /// <summary>
    /// Retrieves a detail model by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the entity.</param>
    /// <returns>The detail model if found; otherwise, null.</returns>
    Task<TDetailModel?> GetAsync(Guid id);
    
    /// <summary>
    /// Retrieves a queryable collection of list models.
    /// </summary>
    /// <returns>A queryable collection of list models.</returns>
    Task<IQueryable<TListModel>> GetQueryAsync();
    
    /// <summary>
    /// Saves a detail model by adding or updating the corresponding entity.
    /// </summary>
    /// <param name="model">The detail model to save.</param>
    Task SaveAsync(TDetailModel model);

    /// <summary>
    /// Saves a collection of detail models by adding or updating the corresponding entities.
    /// </summary>
    /// <param name="models">The collection of detail models to save.</param>
    Task SaveAsync(IEnumerable<TDetailModel> models);
    
    /// <summary>
    /// Deletes an entity by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the entity to delete.</param>
    /// <exception cref="InvalidOperationException">Thrown when the entity cannot be deleted.</exception>
    Task DeleteAsync(Guid id);
}