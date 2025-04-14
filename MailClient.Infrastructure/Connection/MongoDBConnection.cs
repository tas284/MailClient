using MongoDB.Driver;
using Microsoft.Extensions.Options;
using MailClient.Infrastructure.Configuration;
using MailClient.Infrastructure.Interfaces;

namespace MailClient.Infrastructure.Connection
{
    public class MongoDBConnection : IDBConnection
    {
        private readonly MongoDBConfiguration _configuration;
        private readonly MongoClient _client;
        private readonly IMongoDatabase _database;

        public MongoDBConnection(IOptions<MongoDBConfiguration> configuration)
        {
            _configuration = configuration.Value;
            _client = new MongoClient(_configuration.ConnectionString);
            _database = _client.GetDatabase(_configuration.Database);
        }

        public IMongoDatabase Database => _database;

        public IMongoCollection<TDocument> GetCollection<TDocument>(string collection) => _database.GetCollection<TDocument>(collection);
    }
}
