using VisitorService.Application.Interfaces;
using Resend;
using Microsoft.Extensions.Configuration;

public class EmailService : IEmailService
{
    private readonly IResend _resend;

    public EmailService(IConfiguration config) 
    {
        var apiKey = config["ResendSettings:ApiToken"];
        _resend = ResendClient.Create(apiKey);
    }

    public async Task SendAsync(string to, string subject, string body)
{
    var message = new EmailMessage
    {
        From = "onboarding@resend.dev",
        To = to,
        Subject = subject,
        HtmlBody = body
    };
    await _resend.EmailSendAsync(message);
}
}