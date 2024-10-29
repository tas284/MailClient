using MongoDB.Driver;

namespace MailClient.Domain.Interfaces
{
    public interface IConnection
    {
        IMongoDatabase Database { get; }
    }
}
