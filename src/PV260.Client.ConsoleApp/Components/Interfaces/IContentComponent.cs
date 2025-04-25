namespace PV260.Client.ConsoleApp.Components.Interfaces;

internal interface IContentComponent : IRenderableComponent
{
    void HandleInput(ConsoleKeyInfo key);
}