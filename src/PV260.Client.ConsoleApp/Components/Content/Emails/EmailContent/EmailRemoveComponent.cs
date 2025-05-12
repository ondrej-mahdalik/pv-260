using PV260.Client.BL;
using PV260.Client.ConsoleApp.Components.Content.Common;
using PV260.Client.ConsoleApp.Components.Interfaces;
using PV260.Client.ConsoleApp.Components.Navigation.Interfaces;
using PV260.Common.Models;
using Spectre.Console;
using Spectre.Console.Rendering;

namespace PV260.Client.ConsoleApp.Components.Content.Emails.EmailContent;

internal class EmailRemoveComponent(
    IApiClient apiClient,
    INavigationService navigationService) : IAsyncContentComponent
{
    private const string HeaderName = "Email deletion";

    private readonly IApiClient _apiClient = apiClient;
    private readonly INavigationService _navigationService = navigationService;

    public bool IsInSubMenu => false;

    public async Task<IRenderable> RenderAsync()
    {
        try
        {
            var paginationCursor = new PaginationCursor
            {
                PageSize = 1000
            };

            var emailList = await _apiClient.GetAllEmailsAsync(paginationCursor);

            if (!emailList.Items.Any())
            {
                return new EmailContentPanelBuilder()
                    .WithHeader(HeaderName)
                    .WithError("No email recipients found to remove", MessageSize.TableRow)
                    .Build();
            }

            AnsiConsole.Clear();
            var emailToRemove = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("[bold red]Select an email address to remove:[/]")
                    .PageSize(10)
                    .AddChoices(emailList.Items.Select(e => e.EmailAddress)));

            await _apiClient.DeleteEmailAsync(emailToRemove);

            return new EmailContentPanelBuilder()
                .WithHeader(HeaderName)
                .WithSuccess($"Email address '{emailToRemove}' has been removed successfully!", MessageSize.TableRow)
                .Build();
        }
        catch (Exception)
        {
            return new EmailContentPanelBuilder()
                .WithHeader(HeaderName)
                .WithError("Failed to remove email recipient!",
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