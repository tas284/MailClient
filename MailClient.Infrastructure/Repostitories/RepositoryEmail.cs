using MongoDB.Bson;
using MongoDB.Driver;
using DnsClient.Internal;
using MailClient.Domain.Entities;
using MailClient.Domain.Interfaces;
using MailClient.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace MailClient.Infrastructure.Repostitories
{
    public class RepositoryEmail : IRepositoryEmail
    {
        private readonly IConnection _connection;
        private readonly ILogger<RepositoryEmail> _logger;
        private static string RepositoryName = "Email";
        private IMongoCollection<Email> _collection;

        public RepositoryEmail(IConnection connection, ILogger<RepositoryEmail> logger)
        {
            _connection = connection;
            _logger = logger;
            _collection = _connection.Database.GetCollection<Email>(RepositoryName);
        }

        public async Task InsertOneAsync(Email entity)
        {
            try
            {
                await _collection.InsertOneAsync(entity);

                _logger.LogInformation($"Email message successfully entered into the database: {entity.Id}");
            }
            catch (Exception ex)
            {
                _logger.LogInformation($"Error when adding email message to database: {entity.Subject}");
                _logger.LogError($"Error when adding email message to database: {ex.Message}");
            }
        }

        public async Task<Email> GetByIdAsync(string id)
        {
            try
            {
                var filter = Builders<Email>.Filter.Eq(x => x.Id, new ObjectId(id));
                var entity = await _collection.Find(filter).FirstOrDefaultAsync();
                return entity;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error when adding email message to database: {ex.Message}");
                throw new Exception($"Error when adding email message to database: {ex.Message}", ex);
            }
        }
    }
}
