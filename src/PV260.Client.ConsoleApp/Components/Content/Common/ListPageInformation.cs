namespace PV260.Client.ConsoleApp.Components.Content.Common;

internal record ListPageInformation<T> : PageInformation<T> where T : class
{
    public ListPageStack<T> ListPageStack { get; } = new();
}