using Microsoft.Extensions.Logging;
using MailClient.Application.DTO;
using MailClient.Application.Interfaces;
using MailClient.Domain.Repositories;

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
                var entity = await _repository.GetByIdAsync(id);
                if (entity == null) throw new Exception($"Email not found: {id}");
                var email = new EmailDto(entity.Id.ToString(), entity.EmailFrom, entity.Inbox, entity.Subject, entity.Body, entity.Date);
                return email;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error retrieving email from database: {ex.Message}");
                throw new Exception($"Error retrieving email from database: {ex.Message}", ex);
            }
        }
    }
}
