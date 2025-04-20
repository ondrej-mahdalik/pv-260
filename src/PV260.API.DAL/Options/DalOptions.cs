namespace PV260.API.DAL.Options;

/// <summary>
/// Represents the options for configuring the Data Access Layer.
/// </summary>
public record DalOptions
{
    /// <summary>
    /// Indicates whether the database should be recreated.
    /// </summary>
    /// <remarks>
    /// If set to <c>true</c>, the database will be dropped and recreated on application startup.
    /// </remarks>
    public required bool RecreateDatabase { get; init; }
    
    /// <summary>
    /// The connection string used to connect to the database.
    /// </summary>
    public required string ConnectionString { get; init; }
}