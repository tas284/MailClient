using MailClient.API.InputModel;
using System.Threading.Tasks;

namespace MailClient.API.Interfaces
{
    public interface IEmailSmtpService
    {
        string Send(SendEmailInputModel input);
    }
}
