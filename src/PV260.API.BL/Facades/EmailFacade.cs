using PV260.API.BL.Services;

namespace PV260.API.BL.Facades;

//TODO implement subject, body and recipients 
public class EmailFacade(IEmailService emailService) : IEmailFacade
{
    public async Task TestEmailAsync()
    {
        var testEmails = new[] { "test1@gmail.com", "test2@gmail.com", "test3@gmail.com" };
        await emailService.SendEmailAsync(testEmails, "Test Group Email", "Hello, this is a test to multiple recipients.");
    }

    public async Task TestEmailSingleAsync()
    {
        var singleEmail = "test1@gmail.com";
        await emailService.SendEmailAsync(singleEmail, "Test Single Email", "Hello, this is a test to a single recipient.");
    }
}
