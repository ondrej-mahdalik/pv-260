namespace PV260.Common.Models;

/// <inheritdoc />
public abstract record DataModelBase : IDataModel
{
    public Guid Id { get; init; } = Guid.NewGuid();
}