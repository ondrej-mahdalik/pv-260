using Spectre.Console.Rendering;

namespace PV260.Client.ConsoleApp.Components.Interfaces;

internal interface IRenderableComponent
{
    bool IsInSubMenu { get; }
    IRenderable Render();
}