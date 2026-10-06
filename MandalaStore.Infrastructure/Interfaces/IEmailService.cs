namespace MandalaStore.Infrastructure.Interfaces;

public interface IEmailService
{
    Task EnviarAsync(
        string destinatario,
        string assunto,
        string html);
}
