using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System.Text;
using System.IO.Compression;
using MailClient.Infrastructure.Interfaces;

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
            get => CompressString(Body);
            set => Body = DecompressString(value);
        }
        public DateTime Date { get; set; }


        public static byte[] CompressString(string text)
        {
            if (string.IsNullOrEmpty(text)) { return null; }

            byte[] textAsByte = Encoding.UTF8.GetBytes(text);
            using (var memoryStream = new MemoryStream())
            {
                using (var gzipStream = new GZipStream(memoryStream, CompressionMode.Compress))
                {
                    gzipStream.Write(textAsByte, 0, textAsByte.Length);
                }
                return memoryStream.ToArray();
            }
        }

        public static string DecompressString(byte[] compressData)
        {
            if (compressData == null) { return null; }

            using (var memoryStream = new MemoryStream())
            using (var gzipStream = new GZipStream(memoryStream, CompressionMode.Decompress))
            using (var reader = new StreamReader(gzipStream))
            {
                return reader.ReadToEnd();
            }
        }
    }
}
