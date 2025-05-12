namespace PV260.Client.ConsoleApp.Components.Interfaces;

internal interface IAsyncContentComponent : IAsyncRenderableComponent
{
    Task HandleInputAsync(ConsoleKeyInfo key);
}