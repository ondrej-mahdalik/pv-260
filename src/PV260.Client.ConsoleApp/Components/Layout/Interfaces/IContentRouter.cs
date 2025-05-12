using PV260.Client.ConsoleApp.Components.Interfaces;
using PV260.Client.ConsoleApp.Components.Layout.Enums;

namespace PV260.Client.ConsoleApp.Components.Layout.Interfaces;

internal interface IContentRouter
{
    IRenderableComponent Route(MenuItems menuItem);
}