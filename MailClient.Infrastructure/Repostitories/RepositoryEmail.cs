using MongoDB.Bson;
using MongoDB.Driver;
using DnsClient.Internal;
using MailClient.Domain.Entities;
using MailClient.Domain.Repositories;
using Microsoft.Extensions.Logging;
using MailClient.Infrastructure.Interfaces;
using System.Linq.Expressions;

namespace MailClient.Infrastructure.Repositories
{
    public class RepositoryEmail : IRepositoryEmail
    {
        private readonly IDBConnection _connection;
        private readonly IMongoCollection<Email> _collection;
        private readonly ILogger<RepositoryEmail> _logger;

        public RepositoryEmail(IDBConnection connection, ILogger<RepositoryEmail> logger)
        {
            _logger = logger;
            _connection = connection;
            _collection = _connection.GetCollection<Email>(nameof(Email));
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
                var entity = await _collection.Find(FilterById(id)).FirstOrDefaultAsync();
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
                var result = await _collection.DeleteOneAsync(FilterById(id));
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
                var result = await _collection.DeleteManyAsync(_ => true);
                return result.DeletedCount;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error when deleting all email messages from the database: {ex.Message}");
                throw new Exception($"Failed to delete all email messages from the database: {ex.Message}", ex);
            }
        }

        public async Task<long> CountAsync(Expression<Func<Email, bool>> filter) => await _collection.CountDocumentsAsync(filter);

        public async Task UpsertAsync(Email email)
        {
            try
            {
                var options = new FindOneAndReplaceOptions<Email> { IsUpsert = true, ReturnDocument = ReturnDocument.After };
                var filter = Builders<Email>.Filter.Eq(x => x.MessageId, email.MessageId);
                var entity = await _collection.FindOneAndReplaceAsync(filter, email, options);
                _logger.LogInformation($"Email message successfully upserting into the database: {entity.Id}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error when upserting email message to database: {ex.Message}");
                throw new Exception($"Error when upserting email message to database: {ex.Message}", ex);
            }
        }

        public async Task<long> UpsertManyAsync(IEnumerable<Email> emails)
        {
            try
            {
                var result = await _collection.BulkWriteAsync(emails.Select(GetWriteModel));
                return result.RequestCount;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error when inserting all email messages on the database: {ex.Message}");
                throw new Exception($"Error when inserting all email messages on the database: {ex.Message}", ex);
            }
        }

        private WriteModel<Email> GetWriteModel(Email email)
            => new ReplaceOneModel<Email>(Builders<Email>.Filter.Eq(x => x.MessageId, email.MessageId), email) { IsUpsert = true };
        
        private FilterDefinition<Email> FilterById(string id)
            => Builders<Email>.Filter.Eq(x => x.Id, new ObjectId(id));
    }
}
