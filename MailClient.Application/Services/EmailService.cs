using Microsoft.Extensions.Logging;
using MailClient.Application.DTO;
using MailClient.Application.Interfaces;
using MailClient.Domain.Repositories;
using MailClient.Domain.Entities;
using MailClient.Application.Paginator;
using MailClient.Application.Exceptions;

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
                if (string.IsNullOrEmpty(id)) throw new ArgumentException("The id must be informed");

                var entity = await _repository.GetByIdAsync(id);

                return entity is null ? throw new NotFoundException($"Email not found in the database: {id}") : EmailDto.Create(entity);
            }
            catch (Exception ex)
            {
                string message = $"An error occurred while retrieving email from database: {ex.Message}";
                _logger.LogError(message);
                throw;
            }
        }

        public async Task<EmailPaginator> GetAllAsync(int skip, int pageSize)
        {
            try
            {
                if (pageSize <= 0) throw new ArgumentException("The pageSize must be grather than zero");

                var entities = await _repository.GetAllAsync(skip, pageSize);
                if (entities == null || !entities.Any()) throw new NotFoundException("No emails found in the database");

                long total = await _repository.CountAsync();
                var emails = new EmailPaginator(entities, pageSize, skip, total);

                return emails;
            }
            catch (Exception ex)
            {
                string message = $"An error occurred while retrieving all emails from the database: {ex.Message}";
                _logger.LogError(message);
                throw;
            }
        }

        public async Task<string> DeleteByIdAsync(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id)) throw new ArgumentException("The id is required to remove from database");

                var removed = await _repository.DeleteByIdAsync(id);

                return !removed ? throw new NotFoundException($"No email found with this Id {id}") : "Email successfully removed";
            }
            catch (Exception ex)
            {
                string message = $"Error retrieving email from database: {ex.Message}";
                _logger.LogError(message);
                throw;
            }
        }

        public async Task<string> DeleteAllAsync()
        {
            try
            {
                long removed = await _repository.DeleteAllAsync();

                return removed == 0 ? throw new NotFoundException("No emails were found to delete") : $"{removed} emails successfully removed";
            }
            catch (Exception ex)
            {
                string message = $"An error occurred while deleting emails from the database: {ex.Message}";
                _logger.LogError(message);
                throw;
            }
        }
    }
}
