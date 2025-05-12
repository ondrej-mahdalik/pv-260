using Spectre.Console.Rendering;

namespace PV260.Client.ConsoleApp.Components.Interfaces;

internal interface IAsyncRenderableComponent : IRenderableComponent
{
    Task<IRenderable> RenderAsync();
}