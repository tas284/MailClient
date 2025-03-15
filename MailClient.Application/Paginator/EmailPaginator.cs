using MailClient.Application.Abstract;
using MailClient.Application.DTO;
using MailClient.Domain.Entities;

namespace MailClient.Application.Paginator
{
    public class EmailPaginator : AbstractPaginator<EmailDto>
    {
        public EmailPaginator(IEnumerable<Email> emails, int pageSize, int skip, long total)
        {
            PageSize = pageSize;
            NextSkip = skip + pageSize >= total ? 0 : skip + pageSize;
            Total = total;
            Entities = emails.Select(EmailDto.Create);
        }
    }
}
