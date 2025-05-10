namespace PV260.Client.ConsoleApp.Components.Content.Common;

internal record PageStatus
{
    public bool IsSuccess { get; init; } = true;
    public string StatusMessage { get; init; } = string.Empty;
}