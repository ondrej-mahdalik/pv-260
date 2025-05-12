using PV260.Client.ConsoleApp.Components.Interfaces;
using PV260.Client.ConsoleApp.Components.Layout.Interfaces;
using Spectre.Console;
using Spectre.Console.Rendering;

namespace PV260.Client.ConsoleApp.Components.Layout;

internal class LayoutBuilder : ILayoutBuilder
{
    private readonly IFooterComponent _footerComponent;
    private readonly IHeaderComponent _headerComponent;
    private readonly Spectre.Console.Layout _layout;
    private readonly INavbarComponent _navbarComponent;

    private bool _withHeader;
    private bool _withFooter;
    
    private string[]? _navigationMenuItems;
    private int? _navigationSelectedIndex;

    public LayoutBuilder(IHeaderComponent headerComponent, INavbarComponent navbarComponent,
        IFooterComponent footerComponent)
    {
        _headerComponent = headerComponent;
        _navbarComponent = navbarComponent;
        _footerComponent = footerComponent;

        _layout = new Spectre.Console.Layout("Root")
            .SplitRows(
                new Spectre.Console.Layout("Header").Size(3),
                new Spectre.Console.Layout("Body").Ratio(1),
                new Spectre.Console.Layout("Footer").Size(3)
            );

        _layout["Body"].SplitColumns(
            new Spectre.Console.Layout("Sidebar").Size(30),
            new Spectre.Console.Layout("Main")
        );
    }

    public LayoutBuilder WithHeader()
    {
        _withHeader = true;
        return this;
    }

    public LayoutBuilder WithNavigation(string[] menuItems, int selected)
    {
        _navigationMenuItems = menuItems;
        _navigationSelectedIndex = selected;
        return this;
    }

    public LayoutBuilder WithContent(IRenderable content)
    {
        _layout["Main"].Update(content);
        return this;
    }

    public LayoutBuilder WithLoadingContent()
    {
        var content = new Panel(new Markup("[green]Loading...[/]").Centered())
            .Expand();

        _layout["Main"].Update(content);
        return this;
    }

    public LayoutBuilder WithFooter()
    {
        _withFooter = true;
        return this;
    }

    public async Task<IRenderable> BuildAsync()
    {
        if (_withHeader)
        {
            _layout["Header"].Update(_headerComponent.Render());
        }
        
        if (_withFooter)
        {
            _layout["Footer"].Update(_footerComponent.Render());
        }
        
        if (_navigationMenuItems is not null && _navigationSelectedIndex.HasValue)
        {
            _layout["Sidebar"].Update(_navbarComponent.Render(_navigationMenuItems, _navigationSelectedIndex.Value));
        }

        return _layout;
    }
}