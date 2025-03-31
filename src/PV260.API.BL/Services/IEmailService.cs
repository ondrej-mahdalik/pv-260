namespace PV260.API.BL.Services;

public interface IEmailService
{
    Task SendEmailAsync(string to, string subject, string body);
    
    Task SendEmailAsync(IEnumerable<string> to, string subject, string body);
}