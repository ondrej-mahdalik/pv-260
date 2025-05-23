namespace PV260.Client.ConsoleApp.Components.Content.Emails.EmailOptions;

internal enum EmailOptions
{
    ListEmailRecipients,
    AddEmailRecipient,
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
            EmailOptions.ClearEmailRecipients => "Delete All Recipients",
            _ => option.ToString()
        };
    }

    public static string GetDescription(this EmailOptions option)
    {
        return option switch
        {
            EmailOptions.AddEmailRecipient => "Add a new email address to the list of recipients",
            EmailOptions.ListEmailRecipients => "View and manage email recipients",
            EmailOptions.ClearEmailRecipients => "Remove all email recipients",
            _ => option.ToString()
        };
    }
}