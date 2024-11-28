namespace MailClient.Application.Interfaces
{
    public interface ISpecification<T>
    {
        bool IsSatisfiedBy(T entity);
    }
}
