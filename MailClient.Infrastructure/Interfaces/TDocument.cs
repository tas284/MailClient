using MongoDB.Bson;

namespace MailClient.Infrastructure.Interfaces
{
    public interface TDocument
    {
        public ObjectId Id { get; set; }
    }
}
