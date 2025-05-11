using System.Text.RegularExpressions;

namespace PV260.Client.ConsoleApp.Components.Content.Emails;

public static partial class EmailHelper
{
    [GeneratedRegex(@"^\S+@\S+\.\S+$")]
    private static partial Regex EmailRegex();

    public static bool IsValidEmail(string email)
    {
        return EmailRegex().IsMatch(email);
    }
}