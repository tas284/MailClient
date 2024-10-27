using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using System.Text;
using MailClient.Consumer.Configuration;
using MailClient.Consumer.Model;
using Newtonsoft.Json;
using MailClient.Infrastructure.Connection;
using MailClient.Infrastructure.Model;

namespace MailClient.Services
{
    public class ConsumerEmailImapService
    {
        private readonly RabbitMqConfiguration _configuration;
        private readonly ConnectionFactory _factory;
        private readonly ILogger<ConsumerEmailImapService> _logger;
        private readonly DBConnection _connection;

        public ConsumerEmailImapService(IOptions<RabbitMqConfiguration> configuration, ILogger<ConsumerEmailImapService> logger, DBConnection connection)
        {
            _configuration = configuration.Value;
            _factory = new ConnectionFactory { HostName = _configuration.Host };
            _logger = logger;
            _connection = connection;
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

        private void ReadQueue(object sender, BasicDeliverEventArgs eventArgs)
        {
            try
            {
                var body = eventArgs.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                var imapMail = JsonConvert.DeserializeObject<ImapMailModel>(message);

                if (imapMail != null)
                {
                    _logger.LogInformation($"Email received: {imapMail.Subject} from {imapMail.EmailFrom}");

                    _connection.Database().GetCollection<Email>("emails").InsertOne(new Email
                    {
                        Inbox = imapMail.Inbox,
                        EmailFrom = imapMail.EmailFrom,
                        Subject = imapMail.Subject,
                        Body = imapMail.Body,
                        Date = imapMail.Date
                    });
                }

            }
            catch (Exception ex)
            {
                _logger.LogError($"Error processing message: {ex.Message}");
            }
        }
    }
}
