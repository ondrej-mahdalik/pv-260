namespace PV260.API.BL.Mappers;

/// <summary>
/// Defines a mapper interface for converting between entity, list model, and detail model representations.
/// </summary>
/// <typeparam name="TEntity">The type of the entity.</typeparam>
/// <typeparam name="TListModel">The type of the list model.
///     Use same type as <see cref="TDetailModel"/> if you only want to map the entity to one model.</typeparam>
/// <typeparam name="TDetailModel">The type of the detail model.</typeparam>
public interface IMapper<TEntity, out TListModel, TDetailModel>
{
    /// <summary>
    /// Converts an entity to a list model representation.
    /// </summary>
    /// <param name="entity">The entity to convert.</param>
    /// <returns>The list model representation of the entity.</returns>
    TListModel ToListModel(TEntity entity);

    /// <summary>
    /// Converts a collection of entities to a collection of list model representations.
    /// </summary>
    /// <param name="entities">The collection of entities to convert.</param>
    /// <returns>A collection of list model representations.</returns>
    IEnumerable<TListModel> ToListModel(IEnumerable<TEntity> entities)
        => entities.Select(ToListModel);

    /// <summary>
    /// Converts a queryable collection of entities to a queryable collection of list model representations.
    /// </summary>
    /// <param name="entities">The queryable collection of entities to convert.</param>
    /// <returns>A queryable collection of list model representations.</returns>
    IQueryable<TListModel> ToListModel(IQueryable<TEntity> entities)
        => entities.Select(entity => ToListModel(entity));

    /// <summary>
    /// Converts an entity to a detail model representation.
    /// </summary>
    /// <param name="entity">The entity to convert.</param>
    /// <returns>The detail model representation of the entity.</returns>
    TDetailModel ToDetailModel(TEntity entity);

    /// <summary>
    /// Converts a detail model back to an entity representation.
    /// </summary>
    /// <param name="detailModel">The detail model to convert.</param>
    /// <returns>The entity representation of the detail model.</returns>
    TEntity ToEntity(TDetailModel detailModel);
    
    /// <summary>
    /// Converts a collection of detail models to a collection of entity representations.
    /// </summary>
    /// <param name="detailModels">The collection of detail models to convert.</param>
    /// <returns>A collection of entity representations.</returns>
    IEnumerable<TEntity> ToEntity(IEnumerable<TDetailModel> detailModels)
        => detailModels.Select(ToEntity);
}