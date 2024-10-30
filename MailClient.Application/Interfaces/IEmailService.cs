using MailClient.Application.DTO;

namespace MailClient.Application.Interfaces
{
    public interface IEmailService
    {
        Task<EmailDto> GetByIdAsync(string id);
    }
}
