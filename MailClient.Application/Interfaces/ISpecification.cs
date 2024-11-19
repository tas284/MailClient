namespace MailClient.Application.Interfaces
{
    public interface ISpecification<T>
    {
        bool IsSatifiedBy(T entity);
    }
}
