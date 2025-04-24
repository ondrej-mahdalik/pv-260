namespace PV260.Client.ConsoleApp.Components.Interfaces;

internal interface IContentComponent : IRenderableComponent
{
    Task HandleInput(ConsoleKeyInfo key);
}