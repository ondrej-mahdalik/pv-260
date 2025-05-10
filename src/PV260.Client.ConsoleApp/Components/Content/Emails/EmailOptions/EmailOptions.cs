namespace PV260.Client.ConsoleApp.Components.Content.Emails.EmailOptions;

internal enum EmailOptions
{
    ListEmailRecipients,
    AddEmailRecipient,
    RemoveEmailRecipient,
    ClearEmailRecipients
}

internal static class EmailOptionsExtensions
{
    public static string ToFriendlyString(this EmailOptions option)
    {
        return option switch
        {
            EmailOptions.AddEmailRecipient => "Add Recipient",
            EmailOptions.ListEmailRecipients => "List Recipients",
            EmailOptions.RemoveEmailRecipient => "Remove Recipient",
            EmailOptions.ClearEmailRecipients => "Clear All Recipients",
            _ => option.ToString()
        };
    }

    public static string GetDescription(this EmailOptions option)
    {
        return option switch
        {
            EmailOptions.AddEmailRecipient => "Adds a new email address to the list of recipients.",
            EmailOptions.ListEmailRecipients => "Displays the list of currently added email recipients.",
            EmailOptions.RemoveEmailRecipient => "Removes a specific email address from the recipients list.",
            EmailOptions.ClearEmailRecipients => "Removes all email recipients at once.",
            _ => option.ToString()
        };
    }
}