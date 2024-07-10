using MailClient.InputModel;

namespace MailClient.Interfaces
{
    public interface IEmailSmtpService
    {
        Task<string> Send(SendEmailInputModel input);
    }
}
