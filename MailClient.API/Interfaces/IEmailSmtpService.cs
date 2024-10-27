using MailClient.API.InputModel;
using System.Threading.Tasks;

namespace MailClient.API.Interfaces
{
    public interface IEmailSmtpService
    {
        Task<string> Send(SendEmailInputModel input);
    }
}
