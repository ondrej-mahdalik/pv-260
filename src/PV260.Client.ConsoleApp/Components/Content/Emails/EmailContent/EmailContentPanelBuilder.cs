using PV260.Client.ConsoleApp.Components.Content.Common;
using Spectre.Console;
using Spectre.Console.Rendering;

namespace PV260.Client.ConsoleApp.Components.Content.Emails.EmailContent;

internal class EmailContentPanelBuilder : PanelBuilderBase<EmailContentPanelBuilder>
{
    private readonly Table? _contentTable = null;

    protected override void FillContent(List<IRenderable> rows)
    {
        if (_contentTable is not null)
        {
            rows.Add(_contentTable);
        }
    }
}