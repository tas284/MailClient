using MailClient.Domain.Entities;
using MailClient.Domain.Interfaces;
using MailClient.Domain.Repositories;

namespace MailClient.Infrastructure.Repostitories
{
    public class RepositoryEmail : IRepositoryEmail
    {
        private readonly IConnection _connection;
        private static string Collection = "emails";

        public RepositoryEmail(IConnection connection)
        {
            _connection = connection;
        }

        public async Task InsertOneAsync(Email entity)
        {
            try
            {
                await _connection.Database.GetCollection<Email>(Collection).InsertOneAsync(entity);
            }
            catch (Exception ex)
            {
            }
        }
    }
}
