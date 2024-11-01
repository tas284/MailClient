using MailClient.Application.DTO;

namespace MailClient.Application.Interfaces
{
    public interface IEmailService
    {
        Task<EmailDto> GetByIdAsync(string id);
        Task<IEnumerable<EmailDto>> GetAllAsync();
        Task<string> DeleteByIdAsync(string id);
        Task<string> DeleteAllAsync();
    }
}
