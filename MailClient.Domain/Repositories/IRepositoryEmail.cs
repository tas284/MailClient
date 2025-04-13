using MailClient.Domain.Entities;
namespace MailClient.Domain.Repositories
{
    public interface IRepositoryEmail
    {
        Task InsertOneAsync(Email entity);
        Task<Email> GetByIdAsync(string id);
        Task<IEnumerable<Email>> GetAllAsync(int pageSize, int skip);
        Task<bool> DeleteByIdAsync(string id);
        Task<long> DeleteAllAsync();
        Task<long> CountAsync();
        Task<long> InsertManyAsync(IEnumerable<Email> emails);
    }
}