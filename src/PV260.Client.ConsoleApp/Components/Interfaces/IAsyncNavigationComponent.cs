using PV260.Client.ConsoleApp.Components.Navigation.Interfaces;

namespace PV260.Client.ConsoleApp.Components.Interfaces;

internal interface IAsyncNavigationComponent : IAsyncRenderableComponent
{
    int SelectedIndex { get; }
    string[] NavigationItems { get; }
    void Navigate(ConsoleKey key);
    Task HandleInputAsync(ConsoleKey key, INavigationService navigationService);
}