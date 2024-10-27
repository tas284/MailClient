namespace MailClient.Infrastructure.Configuration
{
    public class MongoDBConfiguration
    {
        public string ConnectionString { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Database { get; set; }
    }
}
