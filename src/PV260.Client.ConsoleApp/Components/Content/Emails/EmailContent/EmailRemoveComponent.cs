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
    INavigationService navigationService) : IContentComponent
{
    private const string HeaderName = "Email deletion";

    private readonly IApiClient _apiClient = apiClient;
    private readonly INavigationService _navigationService = navigationService;

    public bool IsInSubMenu => false;

    public IRenderable Render()
    {
        try
        {
            var paginationCursor = new PaginationCursor
            {
                PageSize = 1000
            };

            var emailList =
                _apiClient.GetAllEmailsAsync(paginationCursor).Result; // Synchronous call for simplicity in console apps

            if (!emailList.Items.Any())
            {
                return new EmailContentPanelBuilder()
                    .WithHeader(HeaderName)
                    .WithError("No email recipients found to remove", MessageSize.TableRow)
                    .Build();
            }

            var emailToRemove = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("[bold red]Select an email address to remove:[/]")
                    .PageSize(10)
                    .AddChoices(emailList.Items.Select(e => e.EmailAddress)));

            _apiClient.DeleteEmailAsync(emailToRemove).Wait(); // Synchronous call for simplicity

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

    public void HandleInput(ConsoleKeyInfo key)
    {
    }
}