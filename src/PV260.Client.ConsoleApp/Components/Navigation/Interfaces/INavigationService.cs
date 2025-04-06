using PV260.Client.ConsoleApp.Components.Interfaces;

namespace PV260.Client.ConsoleApp.Components.Navigation.Interfaces;

internal interface INavigationService
{
    IRenderableComponent Current { get; }

    IRenderableComponent? LastNavigationComponent { get; }

    bool CanGoBack { get; }

    void Push(IRenderableComponent component);

    void Pop();
}