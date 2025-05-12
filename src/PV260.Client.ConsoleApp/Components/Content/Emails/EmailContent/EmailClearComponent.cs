using PV260.Client.BL;
using PV260.Client.ConsoleApp.Components.Content.Common;
using PV260.Client.ConsoleApp.Components.Interfaces;
using PV260.Client.ConsoleApp.Components.Navigation.Interfaces;
using Spectre.Console;
using Spectre.Console.Rendering;

namespace PV260.Client.ConsoleApp.Components.Content.Emails.EmailContent;

internal class EmailClearComponent(
    IApiClient apiClient,
    INavigationService navigationService) : IAsyncContentComponent
{
    private const string HeaderName = "Deleting all emails";

    private readonly IApiClient _apiClient = apiClient;
    private readonly INavigationService _navigationService = navigationService;

    public bool IsInSubMenu => false;

    public async Task<IRenderable> RenderAsync()
    {
        try
        {
            AnsiConsole.Clear();
            var confirmation =
                AnsiConsole.Confirm("[bold red]Are you sure you want to remove all email recipients?[/]");

            if (!confirmation)
            {
                return new EmailContentPanelBuilder()
                    .WithHeader(HeaderName)
                    .WithError("Clear operation canceled by user", MessageSize.TableRow)
                    .Build();
            }

            await _apiClient.DeleteAllEmailsAsync();

            return new EmailContentPanelBuilder()
                .WithHeader(HeaderName)
                .WithSuccess("All email recipients have been removed successfully!", MessageSize.TableRow)
                .Build();
        }
        catch (Exception)
        {
            return new EmailContentPanelBuilder()
                .WithHeader(HeaderName)
                .WithError("Failed to clear email recipients!",
                    MessageSize.TableRow)
                .Build();
        }
        finally
        {
            _navigationService.Pop();
        }
    }

    public Task HandleInputAsync(ConsoleKeyInfo key)
    {
        return Task.CompletedTask;
    }
    
    public IRenderable Render()
        => throw new NotSupportedException();
}