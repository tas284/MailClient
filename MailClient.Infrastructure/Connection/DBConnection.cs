using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MailClient.Infrastructure.Configuration;

namespace MailClient.Infrastructure.Connection
{
    public class DBConnection
    {
        private readonly MongoDBConfiguration _configuration;
        private readonly MongoClient _client;
        private readonly IMongoDatabase _database;

        public DBConnection(IOptions<MongoDBConfiguration> configuration)
        {
            _configuration = configuration.Value;
            _client = new MongoClient(_configuration.ConnectionString);
            _database = _client.GetDatabase(_configuration.Database);
        }

        public IMongoDatabase Database()
        {
            return _database;
        }
    }
}
