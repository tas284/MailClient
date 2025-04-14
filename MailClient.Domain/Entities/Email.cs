using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MailClient.Infrastructure.Interfaces;
using MailClient.Domain.Extensions;

namespace MailClient.Domain.Entities
{
    public class Email : TDocument
    {
        [BsonId]
        [BsonIgnoreIfDefault]
        public ObjectId Id { get; set; }
        public string Inbox { get; set; }
        public string EmailFrom { get; set; }
        public string Subject { get; set; }
        public string ExternalId { get; set; }

        [BsonIgnore]
        public string Body
        {
            get => Binary.DecompressString(CompressedBody);
            set => CompressedBody = Binary.CompressString(value);
        }

        public byte[] CompressedBody { get; set; }
        
        public DateTime Date { get; set; }
    }
}
