using MongoDB.Bson;
using MongoDB.Driver;
using DnsClient.Internal;
using MailClient.Domain.Entities;
using MailClient.Domain.Repositories;
using Microsoft.Extensions.Logging;
using MailClient.Infrastructure.Interfaces;

namespace MailClient.Infrastructure.Repositories
{
    public class RepositoryEmail : IRepositoryEmail
    {
        private readonly string Name = "Email";
        private readonly IDBConnection _connection;
        private readonly IMongoCollection<Email> _collection;
        private readonly ILogger<RepositoryEmail> _logger;

        public RepositoryEmail(IDBConnection connection, ILogger<RepositoryEmail> logger)
        {
            _logger = logger;
            _connection = connection;
            _collection = _connection.Database.GetCollection<Email>(Name);
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
                Email entity = await _collection.Find(FilterById(id)).FirstOrDefaultAsync();
                return entity;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error when adding email message to database: {ex.Message}");
                throw new Exception($"Error when adding email message to database: {ex.Message}", ex);
            }
        }

        public async Task<IEnumerable<Email>> GetAllAsync(int skip, int pageSize)
        {
            return await _collection.Find(_ => true).SortByDescending(e => e.Date).Skip(skip).Limit(pageSize).ToListAsync();
        }

        public async Task<bool> DeleteByIdAsync(string id)
        {
            try
            {
                DeleteResult result = await _collection.DeleteOneAsync(FilterById(id));
                return result.IsAcknowledged && result.DeletedCount > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error when deleting email with ID {id} from the database: {ex.Message}");
                throw new Exception($"Failed to delete email with ID {id} from the database: {ex.Message}", ex);
            }
        }

        public async Task<long> DeleteAllAsync()
        {
            try
            {
                DeleteResult result = await _collection.DeleteManyAsync(_ => true);
                return result.DeletedCount;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error when deleting all email messages from the database: {ex.Message}");
                throw new Exception($"Failed to delete all email messages from the database: {ex.Message}", ex);
            }
        }

        private FilterDefinition<Email> FilterById(string id)
        {
            FilterDefinition<Email> filter = Builders<Email>.Filter.Eq(x => x.Id, new ObjectId(id));
            return filter;
        }

        public async Task<long> CountAsync() => await _collection.CountDocumentsAsync(_ => true);
    }
}
