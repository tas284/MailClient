using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using MailClient.Infrastructure.Interfaces;
using MailClient.Domain.Extensions;

namespace MailClient.Domain.Entities
{
    public class Email : TDocument
    {
        [BsonId]
        public ObjectId Id { get; set; }
        public string Inbox { get; set; }
        public string EmailFrom { get; set; }
        public string Subject { get; set; }

        [BsonIgnore]
        public string Body { get; set; }

        [BsonElement("compressedBody")]
        public byte[] CompressedBody
        { 
            get => Binary.CompressString(Body);
            set => Body = Binary.DecompressString(value);
        }
        public DateTime Date { get; set; }
    }
}
