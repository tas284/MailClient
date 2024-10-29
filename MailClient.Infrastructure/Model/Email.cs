using MongoDB.Bson;
using MailClient.Infrastructure.Interfaces;

namespace MailClient.Infrastructure.Model
{
    public class Email : TDocument
    {
        public ObjectId Id { get; set; }
        public string Inbox { get; set; }
        public string EmailFrom { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public DateTime Date { get; set; }
    }
}
