﻿using MailClient.Infrastructure.Configuration;
using MailClient.Infrastructure.Factory;
using MailClient.Infrastructure.Interfaces;
using MailClient.Infrastructure.Model;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System.Text;

namespace MailClient.Infrastructure.Publishers
{
    public class RabbitMqPublisher : IPublisher
    {
        private readonly RabbitMqConfiguration _configuration;
        private readonly ConnectionFactory _factory;
        private readonly ILogger<RabbitMqPublisher> _logger;

        public RabbitMqPublisher(IOptions<RabbitMqConfiguration> configuration, ILogger<RabbitMqPublisher> logger)
        {
            _factory = RabbitMqFactory.CreateConnection(configuration.Value);
            _configuration = configuration.Value;
            _logger = logger;
        }

        public void Publish(object message)
        {
            try
            {
                var imapMailMessage = message as ImapMailMessage;
                using IConnection connection = _factory.CreateConnection();
                using IModel channel = connection.CreateModel();

                channel.QueueDeclare(
                    queue: _configuration.QueueMail,
                    durable: false,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null);

                var stringifiedMessage = JsonConvert.SerializeObject(imapMailMessage);
                var bytesMessage = Encoding.UTF8.GetBytes(stringifiedMessage);

                channel.BasicPublish(
                    exchange: "",
                    routingKey: _configuration.QueueMail,
                    basicProperties: null,
                    body: bytesMessage);

                _logger.LogInformation($"Message published on broker: {imapMailMessage.Subject}.");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Received message was not a ImapMailMessage: {ex.Message}");
            }
        }
    }
}
