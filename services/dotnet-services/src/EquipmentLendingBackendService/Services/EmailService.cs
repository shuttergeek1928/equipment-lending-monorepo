// Services/SmtpNotificationSender.cs
using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;
using SmtpClient = MailKit.Net.Smtp.SmtpClient;

public interface INotificationSender
{
    Task<bool> SendEmailAsync(string to, string subject, string body);
}

public class SmtpNotificationSender : INotificationSender
{
    private readonly EmailSettings _settings;

    public SmtpNotificationSender(IOptions<EmailSettings> settings)
    {
        _settings = settings.Value;
    }

    public async Task<bool> SendEmailAsync(string to, string subject, string body)
    {
        try
        {
            var message = new MimeMessage();
            message.From.Add(MailboxAddress.Parse(_settings.From));
            message.To.Add(MailboxAddress.Parse(to));
            message.Subject = subject;
            message.Body = new TextPart("plain") { Text = body };

            using var client = new SmtpClient();
            await client.ConnectAsync(_settings.Host, _settings.Port, _settings.UseSsl);
            if (!string.IsNullOrEmpty(_settings.User))
                await client.AuthenticateAsync(_settings.User, _settings.Password);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);
            return true;
        }
        catch
        {
            return false;
        }
    }
}

public class EmailSettings
{
    public string Host { get; set; }
    public int Port { get; set; } = 1025;
    public bool UseSsl { get; set; } = false;
    public string From { get; set; }
    public string User { get; set; }
    public string Password { get; set; }
}