using MailClient.Application.InputModel;

namespace MailClient.Application.Interfaces
{
    public interface IEmailSmtpService
    {
        string Send(SendEmailInputModel input);
    }
}
