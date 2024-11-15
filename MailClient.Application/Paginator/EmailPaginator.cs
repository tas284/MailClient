using MailClient.Application.DTO;
using MailClient.Domain.Entities;

namespace MailClient.Application.Paginator
{
    public class EmailPaginator
    {
        public EmailPaginator(IEnumerable<Email> emails, int pageSize, int skip, long total)
        {
            PageSize = pageSize;
            NextSkip = skip + pageSize >= total ? 0 : skip + pageSize;
            Total = total;
            Emails = emails.Select(EmailDto.Create);
        }

        public int PageSize { get; set; }
        public int NextSkip { get; set; }
        public long Total { get; set; }
        public IEnumerable<EmailDto> Emails { get; set; }
    }
}
