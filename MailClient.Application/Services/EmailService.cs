using Microsoft.Extensions.Logging;
using MailClient.Application.DTO;
using MailClient.Application.Interfaces;
using MailClient.Domain.Repositories;
using MailClient.Domain.Entities;

namespace MailClient.Application.Services
{
    public class EmailService : IEmailService
    {
        private readonly ILogger<EmailService> _logger;
        private readonly IRepositoryEmail _repository;

        public EmailService(IRepositoryEmail repository, ILogger<EmailService> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<EmailDto> GetByIdAsync(string id)
        {
            try
            {
                Email entity = await _repository.GetByIdAsync(id);
                if (entity is null) throw new Exception($"Email not found in the database: {id}");
                EmailDto email = EmailDto.Create(entity);
                return email;
            }
            catch (Exception ex)
            {
                string message = $"An error occurred while retrieving email from database: {ex.Message}";
                _logger.LogError(message);
                throw new Exception(message, ex);
            }
        }

        public async Task<IEnumerable<EmailDto>> GetAllAsync()
        {
            try
            {
                IEnumerable<Email> entities = await _repository.GetAllAsync();
                if (entities == null || !entities.Any()) throw new Exception("No emails found in the database.");
                return entities.Select(EmailDto.Create);
            }
            catch (Exception ex)
            {
                string message = $"An error occurred while retrieving all emails from the database: {ex.Message}";
                _logger.LogError(message);
                throw new Exception(message, ex);
            }
        }

        public async Task<string> DeleteByIdAsync(string id)
        {
            try
            {
                bool removed = await _repository.DeleteByIdAsync(id);
                return removed ? "Email successfully removed." : "No email found with this Id.";
            }
            catch (Exception ex)
            {
                string message = $"Error retrieving email from database: {ex.Message}";
                _logger.LogError(message);
                throw new Exception(message, ex);
            }
        }

        public async Task<string> DeleteAllAsync()
        {
            try
            {
                long removed = await _repository.DeleteAllAsync();
                return removed > 0
                    ? $"{removed} emails successfully removed."
                    : "No emails were found to delete.";
            }
            catch (Exception ex)
            {
                string message = $"An error occurred while deleting emails from the database: {ex.Message}";
                _logger.LogError(message);
                throw new Exception(message, ex);
            }
        }
    }
}
