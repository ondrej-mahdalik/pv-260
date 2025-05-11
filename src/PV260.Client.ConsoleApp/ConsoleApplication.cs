using PV260.Client.ConsoleApp.Components.Enums;
using PV260.Client.ConsoleApp.Components.Interfaces;
using PV260.Client.ConsoleApp.Components.Navigation.Interfaces;
using Spectre.Console;

namespace PV260.Client.ConsoleApp;

internal class ConsoleApplication(
    ILayoutBuilder layoutBuilder,
    IContentRouter contentRouter,
    INavigationService navigationService)
{
    private readonly IContentRouter _contentRouter = contentRouter;
    private readonly ILayoutBuilder _layoutBuilder = layoutBuilder;

    private readonly MenuItems[] _mainMenuItems =
    [
        MenuItems.Home, MenuItems.LatestGeneratedReport, MenuItems.Reports, MenuItems.Emails, MenuItems.About
    ];

    private readonly INavigationService _navigationService = navigationService;

    private int _mainSelected;
    private bool _running = true;

    public void Run()
    {
        _navigationService.Push(_contentRouter.Route(MenuItems.Home));

        while (_running)
        {
            var component = _navigationService.Current;

            AnsiConsole.Clear();

            var layout = _layoutBuilder
                .WithHeader()
                .WithContent(component.Render())
                .WithNavigation(GetCurrentNavItems(), GetCurrentSelectedIndex())
                .WithFooter()
                .Build();

            AnsiConsole.Write(layout);

            var key = Console.ReadKey(true);

            DelegateNavigation(component, key);
        }

        AnsiConsole.Clear();
        AnsiConsole.MarkupLine("[green]Thanks for using PV260 Report Generator![/]");
    }

    private void DelegateNavigation(IRenderableComponent component, ConsoleKeyInfo key)
    {
        if (key.Key == ConsoleKey.Escape)
        {
            _running = false;
            return;
        }

        switch (component)
        {
            case INavigationComponent navigationComponent:
            {
                navigationComponent.Navigate(key.Key);
                navigationComponent.HandleInput(key.Key, _navigationService);

                if (key.Key != ConsoleKey.Backspace)
                {
                    return;
                }

                if (_navigationService.CanGoBack)
                {
                    _navigationService.Pop();
                }

                break;
            }
            case IContentComponent when
                _navigationService.LastNavigationComponent is INavigationComponent navigationComponent:
                navigationComponent.HandleInput(key.Key, _navigationService);
                break;
            default:
            {
                if (!component.IsInSubMenu)
                {
                    HandleMainNavigation(key);
                }

                break;
            }
        }
    }

    private void HandleMainNavigation(ConsoleKeyInfo key)
    {
        switch (key.Key)
        {
            case ConsoleKey.UpArrow:
                _mainSelected = (_mainSelected - 1 + _mainMenuItems.Length) % _mainMenuItems.Length;
                break;
            case ConsoleKey.DownArrow:
                _mainSelected = (_mainSelected + 1) % _mainMenuItems.Length;
                break;
            case ConsoleKey.Enter:
                _navigationService.Push(_contentRouter.Route(_mainMenuItems[_mainSelected]));
                break;
            case ConsoleKey.Backspace:
                if (_navigationService.CanGoBack)
                {
                    _navigationService.Pop();
                }

                break;
        }
    }

    private string[] GetCurrentNavItems()
    {
        var current = _navigationService.Current;

        return current switch
        {
            INavigationComponent currentNavigationComponent when currentNavigationComponent.NavigationItems.Any() =>
                currentNavigationComponent.NavigationItems,
            IContentComponent when
                _navigationService.LastNavigationComponent is INavigationComponent navigationComponent &&
                navigationComponent.NavigationItems.Any() => navigationComponent.NavigationItems,
            _ => _mainMenuItems.Any() ? _mainMenuItems.Select(m => m.ToString()).ToArray() : []
        };
    }

    private int GetCurrentSelectedIndex()
    {
        return _navigationService.Current switch
        {
            INavigationComponent nav => nav.SelectedIndex,
            IContentComponent when
                _navigationService.LastNavigationComponent is INavigationComponent navigationComponent =>
                navigationComponent.SelectedIndex,
            _ => _mainSelected
        };
    }
}