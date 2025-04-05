namespace PV260.Common.Models;

/// <summary>
/// Represents a data model with a unique identifier.
/// </summary>
public interface IDataModel
{
    public Guid Id { get; init; }
}