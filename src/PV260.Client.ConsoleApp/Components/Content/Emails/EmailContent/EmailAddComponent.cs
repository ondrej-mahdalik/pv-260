using PV260.Client.BL;
using PV260.Client.ConsoleApp.Components.Content.Common;
using PV260.Client.ConsoleApp.Components.Interfaces;
using PV260.Client.ConsoleApp.Components.Navigation.Interfaces;
using PV260.Common.Models;
using Spectre.Console;
using Spectre.Console.Rendering;

namespace PV260.Client.ConsoleApp.Components.Content.Emails.EmailContent;

internal class EmailAddComponent(
    IApiClient apiClient,
    INavigationService navigationService) : IAsyncContentComponent
{
    private const string HeaderName = "Email creation";

    public bool IsInSubMenu => false;

    public async Task<IRenderable> RenderAsync()
    {
        navigationService.Pop();

        AnsiConsole.Clear();
        var emailAddress = AnsiConsole.Prompt(
            new TextPrompt<string>("[bold green]Enter the email address to add:[/]").Validate(email =>
                EmailHelper.IsValidEmail(email) switch
                {
                    true => ValidationResult.Success(),
                    false => ValidationResult.Error("The email is invalid, please try again")
                }));

        var emailRecipient = new EmailRecipientModel
        {
            EmailAddress = emailAddress,
            CreatedAt = DateTime.UtcNow
        };

        var addEmailResult = await apiClient.AddEmailAsync(emailRecipient);
        if (addEmailResult.IsError)
        {
            return new EmailContentPanelBuilder()
                .WithHeader(HeaderName)
                .WithError("Failed to add email recipient! Verify that email does not already exist!",
                    MessageSize.TableRow)
                .Build();
        }

        return new EmailContentPanelBuilder()
            .WithHeader(HeaderName)
            .WithSuccess($"Email address '{emailAddress}' has been added successfully!", MessageSize.TableRow)
            .Build();
    }

    public Task HandleInputAsync(ConsoleKeyInfo key)
    {
        return Task.CompletedTask;
    }
    
    public IRenderable Render()
        => throw new NotSupportedException();
}