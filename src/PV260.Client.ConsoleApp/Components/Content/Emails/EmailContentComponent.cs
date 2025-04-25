using PV260.Client.BL;
using PV260.Client.ConsoleApp.Components.Interfaces;
using PV260.Client.ConsoleApp.Components.Navigation.Interfaces;
using PV260.Common.Models;
using Spectre.Console;
using Spectre.Console.Rendering;
using System.Text.RegularExpressions;

namespace PV260.Client.ConsoleApp.Components.Content.Emails;

internal class EmailContentComponent(EmailOptions emailOption, IApiClient apiClient, INavigationService navigationService) : IContentComponent
{
    private readonly EmailOptions _emailOption = emailOption;
    private readonly IApiClient _apiClient = apiClient;
    private readonly INavigationService _navigationService = navigationService;

    public bool IsInSubMenu => false;

    public IRenderable Render()
    {
        return _emailOption switch
        {
            EmailOptions.ListEmailRecipients => RenderEmailList(),
            EmailOptions.AddEmailRecipient => RenderAddEmail(),
            EmailOptions.RemoveEmailRecipient => RenderRemoveEmail(),
            EmailOptions.ClearEmailRecipients => RenderClearEmail(),
            _ => new Panel($"[bold cyan]You have selected: {_emailOption}[/]")
                .Border(BoxBorder.Rounded)
                .Expand()
        };
    }

    private Panel RenderEmailList()
    {
        var emailList = _apiClient.GetAllEmailsAsync().Result.ToList(); // Synchronous call for simplicity in console apps

        if (!emailList.Any())
        {
            return new Panel("[yellow]No email recipients found.[/]")
                .Border(BoxBorder.Rounded)
                .Expand();
        }

        var table = new Table()
            .AddColumn("[bold]Email Address[/]")
            .AddColumn("[bold]Created At[/]");

        foreach (var email in emailList)
        {
            table.AddRow(email.EmailAddress, email.CreatedAt.ToString("g"));
        }

        return new Panel(table)
            .Border(BoxBorder.Rounded)
            .Expand();
    }

    private Panel RenderAddEmail()
    {
        try
        {
            var emailAddress = AnsiConsole.Ask<string>("[bold green]Enter the email address to add:[/]");
            if (!IsValidEmail(emailAddress))
            {
                throw new ArgumentException("Invalid email address.");
            }

            var emailRecipient = new EmailRecipientModel
            {
                EmailAddress = emailAddress,
                CreatedAt = DateTime.UtcNow
            };

            _apiClient.AddEmailAsync(emailRecipient).Wait(); // Synchronous call for simplicity
            return new Panel($"[green]Email address '{emailAddress}' has been added successfully![/]")
                .Border(BoxBorder.Rounded)
                .Expand();
        }
        catch (Exception ex)
        {
            return new Panel($"[red]Failed to add email recipient: {ex.Message}[/]")
                .Border(BoxBorder.Rounded)
                .Expand();
        }
        finally
        {
            _navigationService.Pop(); // Return to the email options menu
        }
    }

    private Panel RenderRemoveEmail()
    {
        try
        {
            var emailList = _apiClient.GetAllEmailsAsync().Result.ToList(); // Synchronous call for simplicity in console apps
            if (!emailList.Any())
            {
                throw new InvalidOperationException("No email recipients found to remove.");
            }

            var emailToRemove = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("[bold red]Select an email address to remove:[/]")
                    .PageSize(10)
                    .AddChoices(emailList.Select(e => e.EmailAddress)));

            _apiClient.DeleteEmailAsync(emailToRemove).Wait(); // Synchronous call for simplicity
            return new Panel($"[green]Email address '{emailToRemove}' has been removed successfully![/]")
                .Border(BoxBorder.Rounded)
                .Expand();
        }
        catch (Exception ex)
        {
            return new Panel($"[red]Failed to remove email recipient: {ex.Message}[/]")
                .Border(BoxBorder.Rounded)
                .Expand();
        }
        finally
        {
            _navigationService.Pop(); // Return to the email options menu
        }
    }

    private Panel RenderClearEmail()
    {
        try
        {
            var confirmation = AnsiConsole.Confirm("[bold red]Are you sure you want to remove all email recipients?[/]");
            if (!confirmation)
            {
                throw new OperationCanceledException("Operation canceled by the user.");
            }

            _apiClient.DeleteAllEmailsAsync().Wait(); // Synchronous call for simplicity
            return new Panel("[green]All email recipients have been removed successfully![/]")
                .Border(BoxBorder.Rounded)
                .Expand();
        }
        catch (Exception ex)
        {
            return new Panel($"[red]Failed to clear email recipients: {ex.Message}[/]")
                .Border(BoxBorder.Rounded)
                .Expand();
        }
        finally
        {
            _navigationService.Pop(); // Return to the email options menu
        }
    }

    private bool IsValidEmail(string email)
    {
        return Regex.IsMatch(email, @"^\S+@\S+\.\S+$");
    }

    public void HandleInput(ConsoleKeyInfo key)
    {
        if (key.Key == ConsoleKey.Backspace)
        {
            _navigationService.Pop(); // Return to the email options menu
        }
    }
}

