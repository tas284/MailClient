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
        private static string Collection = "Email";
        private readonly ILogger<RepositoryEmail> _logger;

        public RepositoryEmail(IConnection connection, ILogger<RepositoryEmail> logger)
        {
            _connection = connection;
            _logger = logger;
        }

        public async Task InsertOneAsync(Email entity)
        {
            try
            {
                await _connection.Database.GetCollection<Email>(Collection).InsertOneAsync(entity);

                _logger.LogInformation($"Email message successfully entered into the database: {entity.Id}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error when adding email message to database: {entity.Id}");
            }
        }

        public async Task<Email> GetByIdAsync(string id)
        {
            try
            {
                var filter = Builders<Email>.Filter.Eq(x => x.Id, new ObjectId(id));
                var entity = await _connection.Database.GetCollection<Email>(Collection).Find(filter).FirstOrDefaultAsync();
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
