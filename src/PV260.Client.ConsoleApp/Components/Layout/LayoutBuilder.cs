using PV260.Client.ConsoleApp.Components.Interfaces;
using Spectre.Console;
using Spectre.Console.Rendering;

namespace PV260.Client.ConsoleApp.Components;

internal class LayoutBuilder : ILayoutBuilder
{
    private readonly IFooterComponent _footerComponent;
    private readonly IHeaderComponent _headerComponent;
    private readonly Layout _layout;
    private readonly INavbarComponent _navbarComponent;

    public LayoutBuilder(IHeaderComponent headerComponent, INavbarComponent navbarComponent,
        IFooterComponent footerComponent)
    {
        _headerComponent = headerComponent;
        _navbarComponent = navbarComponent;
        _footerComponent = footerComponent;

        _layout = new Layout("Root")
            .SplitRows(
                new Layout("Header").Size(3),
                new Layout("Body").Ratio(1),
                new Layout("Footer").Size(3)
            );

        _layout["Body"].SplitColumns(
            new Layout("Sidebar").Size(30),
            new Layout("Main")
        );
    }

    public LayoutBuilder WithHeader()
    {
        _layout["Header"].Update(_headerComponent.Render());
        return this;
    }

    public LayoutBuilder WithNavigation(string[] menuItems, int selected)
    {
        _layout["Sidebar"].Update(_navbarComponent.Render(menuItems, selected));
        return this;
    }

    public LayoutBuilder WithContent(IRenderable content)
    {
        _layout["Main"].Update(content);
        return this;
    }

    public LayoutBuilder WithFooter()
    {
        _layout["Footer"].Update(_footerComponent.Render());
        return this;
    }

    public IRenderable Build()
    {
        return _layout;
    }
}