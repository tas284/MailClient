using MailClient.Application.DTO;
using MailClient.Application.Paginator;

namespace MailClient.Application.Interfaces
{
    public interface IEmailService
    {
        Task<EmailDto> GetByIdAsync(string id);
        Task<EmailPaginator> GetAllAsync(int skip, int page);
        Task<string> DeleteByIdAsync(string id);
        Task<string> DeleteAllAsync();
    }
}
