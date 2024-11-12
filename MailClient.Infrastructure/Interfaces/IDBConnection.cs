using MongoDB.Driver;

namespace MailClient.Infrastructure.Interfaces
{
    public interface IDBConnection
    {
        IMongoDatabase Database { get; }
        IMongoCollection<TDocument> GetCollection<TDocument>(string name);
    }
}
