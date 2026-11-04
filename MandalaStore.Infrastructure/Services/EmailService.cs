

using MailKit.Net.Smtp;
using MailKit.Security;
using MandalaStore.Infrastructure.Interfaces;
using MandalaStore.Infrastructure.Settings;
using Microsoft.Extensions.Options;
using MimeKit;

namespace MandalaStore.Infrastructure.Services;

public class EmailService : IEmailService
{
    private readonly SmtpSettings _settings;

    public EmailService(IOptions<SmtpSettings> options)
    {
        _settings = options.Value;
    }

    public async Task EnviarAsync(
        string destinatario,
        string assunto,
        string html)
    {
        try
        {

            var mensagem = new MimeMessage();

        mensagem.From.Add(
            new MailboxAddress(
                _settings.FromName,
                _settings.User));

        mensagem.To.Add(
            MailboxAddress.Parse(destinatario));

        mensagem.Subject = assunto;

        mensagem.Body = new BodyBuilder
        {
            HtmlBody = html
        }.ToMessageBody();

        using var smtp = new SmtpClient();

        await smtp.ConnectAsync(
            _settings.Host,
            _settings.Port,
            SecureSocketOptions.StartTls);

        await smtp.AuthenticateAsync(
            _settings.User,
            _settings.Password);

        await smtp.SendAsync(mensagem);

        await smtp.DisconnectAsync(true);
        
        }
        catch(Exception ex)
        {
            throw;
        }

    }

}