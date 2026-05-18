using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using WWN.Application.Email;

namespace WWN.Infrastructure.Email;

public class SmtpEmailSender : IEmailSender
{
    private readonly SmtpOptions _options;
    private readonly ILogger<SmtpEmailSender> _logger;

    public SmtpEmailSender(IOptions<SmtpOptions> options, ILogger<SmtpEmailSender> logger)
    {
        _options = options.Value;
        _logger = logger;
    }

    public async Task SendAsync(string to, string subject, string htmlBody, CancellationToken cancellationToken = default)
    {
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(_options.FromName, _options.FromAddress));
        message.To.Add(MailboxAddress.Parse(to));
        message.Subject = subject;
        message.Body = new BodyBuilder { HtmlBody = htmlBody }.ToMessageBody();

        using var client = new SmtpClient();
        // Auto lets MailKit pick the right TLS mode based on port:
        // 465 → SslOnConnect (implicit TLS/SMTPS), 587 → StartTls (explicit STARTTLS).
        // Falling back to None when EnableSsl=false keeps plaintext/dev setups working.
        var socketOption = _options.EnableSsl ? SecureSocketOptions.Auto : SecureSocketOptions.None;
        await client.ConnectAsync(_options.Host, _options.Port, socketOption, cancellationToken);

        if (!string.IsNullOrEmpty(_options.Username))
        {
            await client.AuthenticateAsync(_options.Username, _options.Password, cancellationToken);
        }

        await client.SendAsync(message, cancellationToken);
        await client.DisconnectAsync(true, cancellationToken);

        _logger.LogInformation("Sent email to {Recipient} (subject: {Subject})", to, subject);
    }
}
