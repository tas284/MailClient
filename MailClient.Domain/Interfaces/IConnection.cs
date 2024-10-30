using MailClient.Infrastructure.Interfaces;
using MongoDB.Driver;

namespace MailClient.Domain.Interfaces
{
    public interface IConnection
    {
        IMongoDatabase Database { get; }
        IMongoCollection<TDocument> GetCollection<TDocument>(string name);
    }
}
