using MailClient.Domain.Entities;
namespace MailClient.Domain.Repositories
{
    public interface IRepositoryEmail
    {
        Task InsertOneAsync(Email entity);
        Task<Email> GetByIdAsync(string id);
    }
}