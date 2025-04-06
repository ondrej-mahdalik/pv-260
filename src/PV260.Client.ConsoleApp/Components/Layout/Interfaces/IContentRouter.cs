using PV260.Client.ConsoleApp.Components.Enums;

namespace PV260.Client.ConsoleApp.Components.Interfaces;

internal interface IContentRouter
{
    IRenderableComponent Route(MenuItems menuItem);
}