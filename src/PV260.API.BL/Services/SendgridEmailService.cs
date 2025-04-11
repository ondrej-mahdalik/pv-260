using Microsoft.Extensions.Configuration;
using SendGrid;

namespace PV260.API.BL.Services;

public class SendgridEmailService : IEmailService
{
    private const string ReportEmailSenderEmailKey = "EmailConfiguration:ReportEmailSenderEmail";
    private const string ReportEmailSenderNameKey = "EmailConfiguration:ReportEmailSenderName";
    private const string SendGridApiKey = "SendGridApiKey";
    
    private readonly SendGridClient _sendGridClient;
    private readonly string _senderEmail;
    private readonly string _senderName;
    
    public SendgridEmailService(IConfiguration configuration)
    {
        _senderName = configuration[ReportEmailSenderNameKey] ??
                      throw new Exception($"Missing configuration key: {ReportEmailSenderNameKey}");
        _senderEmail = configuration[ReportEmailSenderEmailKey] ??
                       throw new Exception($"Missing configuration key: {ReportEmailSenderEmailKey}");
        
        _sendGridClient = new SendGridClient(configuration[SendGridApiKey]) 
                          ?? throw new Exception("Missing SendGrid API key");
    }

    public Task SendEmailAsync(string to, string subject, string body)
    {
        throw new NotImplementedException();
    }

    public Task SendEmailAsync(IEnumerable<string> to, string subject, string body)
    {
        throw new NotImplementedException();
    }
}