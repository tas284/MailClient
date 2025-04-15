using MailClient.Domain.Entities;
using System.Linq.Expressions;
namespace MailClient.Domain.Repositories
{
    public interface IRepositoryEmail
    {
        Task InsertOneAsync(Email entity);
        Task<Email> GetByIdAsync(string id);
        Task<IEnumerable<Email>> GetAllAsync(int pageSize, int skip);
        Task<bool> DeleteByIdAsync(string id);
        Task<long> DeleteAllAsync();
        Task<long> CountAsync(Expression<Func<Email, bool>> filter);
        Task<long> UpsertManyAsync(IEnumerable<Email> emails);
        Task UpsertAsync(Email email);
    }
}