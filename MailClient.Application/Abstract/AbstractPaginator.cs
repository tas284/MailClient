namespace MailClient.Application.Abstract
{
    public abstract class AbstractPaginator<T>
    {
        public int PageSize { get; set; }
        public int NextSkip { get; set; }
        public long Total { get; set; }
        public IEnumerable<T> Entities { get; set; }
    }
}
