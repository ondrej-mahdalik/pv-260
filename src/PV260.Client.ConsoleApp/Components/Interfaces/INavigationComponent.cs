using PV260.Client.ConsoleApp.Components.Navigation.Interfaces;

namespace PV260.Client.ConsoleApp.Components.Interfaces;

internal interface INavigationComponent : IRenderableComponent
{
    int SelectedIndex { get; }
    string[] NavigationItems { get; }
    void Navigate(ConsoleKey key);
    void HandleInput(ConsoleKey key, INavigationService navigationService);
}