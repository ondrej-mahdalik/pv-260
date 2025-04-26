using Spectre.Console;
using Spectre.Console.Rendering;

namespace PV260.Client.ConsoleApp.Components.Content.Common;

internal abstract class PanelBuilderBase<TBuilder> where TBuilder : PanelBuilderBase<TBuilder>
{
    protected string Header = string.Empty;

    protected Panel? Message;

    public TBuilder WithHeader(string header)
    {
        Header = header;
        return (TBuilder)this;
    }

    public TBuilder WithError(string errorMessage, MessageSize messageSize)
    {
        return WithMessage(errorMessage, Color.Red, "Error", messageSize);
    }

    public TBuilder WithSuccess(string successMessage, MessageSize messageSize)
    {
        return WithMessage(successMessage, Color.Green, "Success", messageSize);
    }

    public TBuilder WithMessage(string message, MessageSize messageSize)
    {
        return WithMessage(message, Color.Silver, "Message", messageSize);
    }

    protected TBuilder WithMessage(string message, Color color, string title, MessageSize messageSize)
    {
        if (!string.IsNullOrEmpty(message))
        {
            Message = new Panel(
                    new Markup($"[bold {color.ToMarkup()}]{message}[/]").Centered())
                .Header($"[{color.ToMarkup()}]{title}[/]", Justify.Center)
                .BorderColor(color)
                .Expand()
                .Padding(new Padding(0));

            if (messageSize == MessageSize.TableRow)
            {
                const int tableRowSize = 3;
                Message.Height = tableRowSize;
            }
        }

        return (TBuilder)this;
    }

    public IRenderable Build()
    {
        var rows = new List<IRenderable>();

        FillContent(rows);

        if (Message is not null)
        {
            rows.Add(Message);
        }

        var layout = new Rows(rows);

        var panel = new Panel(layout)
            .Border(BoxBorder.Rounded)
            .Expand();

        if (!string.IsNullOrEmpty(Header))
        {
            panel.Header($"[bold green]{Header}[/]", Justify.Center);
        }

        return panel;
    }

    protected abstract void FillContent(List<IRenderable> rows);
}