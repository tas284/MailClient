using MailClient.Application.DTO;

namespace MailClient.Application.Paginator
{
    public class EmailPaginator
    {
        public int PageSize { get; set; }
        public int NextSkip { get; set; }
        public long Total { get; set; }
        public IEnumerable<EmailDto> Emails { get; set; }
    }
}
