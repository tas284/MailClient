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
    }
}
