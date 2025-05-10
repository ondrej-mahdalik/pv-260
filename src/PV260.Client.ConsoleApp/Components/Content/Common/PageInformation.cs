namespace PV260.Client.ConsoleApp.Components.Content.Common;

internal record PageInformation<T> where T : class
{
    public int PageSize { get; set; }
    public int SelectedPageIndex { get; set; }
    public T? SelectedModel { get; set; }
}