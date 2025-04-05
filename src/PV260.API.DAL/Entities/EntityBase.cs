namespace PV260.API.DAL.Entities;

public abstract record EntityBase : IEntity
{
    public Guid Id { get; init; } = Guid.NewGuid();
}