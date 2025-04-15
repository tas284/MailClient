using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using Newtonsoft.Json;
using System.Text;
using MailClient.Consumer.Model;
using MailClient.Domain.Repositories;
using MailClient.Domain.Entities;
using MailClient.Consumer.Interfaces;
using MailClient.Infrastructure.Configuration;
using MailClient.Infrastructure.Factory;

namespace MailClient.Services
{
    public class ConsumerEmailImapService : IConsumerEmailImapService
    {
        private readonly RabbitMqConfiguration _configuration;
        private readonly ConnectionFactory _factory;
        private readonly ILogger<ConsumerEmailImapService> _logger;
        private readonly IRepositoryEmail _repository;

        public ConsumerEmailImapService(IOptions<RabbitMqConfiguration> configuration, ILogger<ConsumerEmailImapService> logger, IRepositoryEmail repository)
        {
            _factory = RabbitMqFactory.CreateConnection(configuration.Value);
            _configuration = configuration.Value;
            _logger = logger;
            _repository = repository;
        }

        public async Task ExecuteAsync()
        {
            var connection = _factory.CreateConnection();
            using var channel = connection.CreateModel();

            channel.QueueDeclare(_configuration.QueueMail, durable: false, exclusive: false, autoDelete: false, arguments: null);

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += ReadQueue;

            channel.BasicConsume(queue: _configuration.QueueMail, autoAck: true, consumer: consumer);

            _logger.LogInformation("Consumer started. Waiting for messages...");
            await Task.Delay(-1);
        }

        private async void ReadQueue(object sender, BasicDeliverEventArgs eventArgs)
        {
            try
            {
                var body = eventArgs.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                var entity = JsonConvert.DeserializeObject<ImapMailMessage>(message);

                if (entity != null)
                {
                    _logger.LogInformation($"Email received: {entity.Subject} from {entity.EmailFrom}");

                    var email = new Email
                    {
                        Inbox = entity.EmailTo,
                        EmailFrom = entity.EmailFrom,
                        Subject = entity.Subject,
                        Body = entity.Body,
                        MessageId = entity.MessageId,
                        Date = entity.Date
                    };
                    await _repository.UpsertAsync(email);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error processing message: {ex.Message}");
            }
        }
    }
}