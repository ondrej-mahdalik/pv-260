using PV260.API.BL.Services;

namespace PV260.API.BL.Facades;

public class EmailFacade(IEmailService emailService) : IEmailFacade
{
    public async Task TestEmailAsync()
    {
        await emailService.SendEmailAsync("m.justik@hotmail.com", "Test", "Test");
    }
}