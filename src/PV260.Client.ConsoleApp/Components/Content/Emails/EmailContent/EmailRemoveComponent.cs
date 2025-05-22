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

    public bool IsInSubMenu => false;

    public async Task<IRenderable> RenderAsync()
    {
        navigationService.Pop();
        
        var paginationCursor = new PaginationCursor
        {
            PageSize = 1000
        };

        var emailList = await apiClient.GetAllEmailsAsync(paginationCursor);
        if (emailList.IsError)
        {
            return new EmailContentPanelBuilder()
                .WithHeader(HeaderName)
                .WithError("Failed to retrieve email recipients!", MessageSize.TableRow)
                .Build();
        }

        if (!emailList.Value.Items.Any())
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
                .AddChoices(emailList.Value.Items.Select(e => e.EmailAddress)));

        var emailDeleted = await apiClient.DeleteEmailAsync(emailToRemove);
        if (emailDeleted.IsError)
        {
            return new EmailContentPanelBuilder()
                .WithHeader(HeaderName)
                .WithError("Failed to remove email recipient!", MessageSize.TableRow)
                .Build();
        }

        return new EmailContentPanelBuilder()
            .WithHeader(HeaderName)
            .WithSuccess($"Email address '{emailToRemove}' has been removed successfully!", MessageSize.TableRow)
            .Build();
    }

    public Task HandleInputAsync(ConsoleKeyInfo key)
    {
        return Task.CompletedTask;
    }
    
    public IRenderable Render()
        => throw new NotSupportedException();
}