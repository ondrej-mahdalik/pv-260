using Spectre.Console.Rendering;

namespace PV260.Client.ConsoleApp.Components.Interfaces;

internal interface ILayoutBuilder
{
    LayoutBuilder WithHeader();
    LayoutBuilder WithNavigation(string[] menuItems, int selected);
    LayoutBuilder WithContent(IRenderable content);
    LayoutBuilder WithFooter();
    IRenderable Build();
}