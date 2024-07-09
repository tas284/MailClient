using MailClient.Configuration;
using MailClient.InputModel;
using MailClient.Interfaces;
using MailKit.Net.Imap;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System.Text;

namespace MailClient.Services
{
    public class MailService : IMailService
    {
        private readonly RabbitMqConfiguration _configuration;
        private readonly ConnectionFactory _factory;

        public MailService(IOptions<RabbitMqConfiguration> configuration)
        {
            _configuration = configuration.Value;
            _factory = new ConnectionFactory { HostName = _configuration.Host };
        }

        public Task<bool> SyncEmail(SyncEmailInputModel input)
        {
            using (var client = new ImapClient())
            {
                client.Connect(input.ImapAddress, input.ImapPort, true);

                try
                {
                    client.Authenticate(input.User, input.Password);
                    var inbox = client.Inbox;
                    inbox.Open(MailKit.FolderAccess.ReadOnly);

                    for (int i = inbox.Count; i > 0; i--)
                    {
                        var message = inbox.GetMessage(i - 1);
                        Handle(input.User, message.From.Mailboxes.FirstOrDefault()!.Address, message.Subject, message.HtmlBody, message.Date.LocalDateTime);
                    }
                }
                catch (Exception)
                {
                    return Task.FromResult(false);
                }

                client.Disconnect(true);
            }
            return Task.FromResult(true);
        }

        public Task Handle(string inbox, string emailFrom, string subject, string body, DateTime date)
        {
            return Publish(new InputImapMail(inbox, emailFrom, subject, body, date));
        }

        private Task Publish(object message)
        {
            using (var connection = _factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    channel.QueueDeclare(
                        queue: _configuration.QueueMail,
                        durable: false,
                        exclusive: false,
                        autoDelete: false,
                        arguments: null);

                    string stringfiedMessage = JsonConvert.SerializeObject(message);
                    byte[] bytesMessage = Encoding.UTF8.GetBytes(stringfiedMessage);

                    channel.BasicPublish(
                        exchange: "",
                        routingKey: _configuration.QueueMail,
                        basicProperties: null,
                        body: bytesMessage);
                }
            }

            return Task.CompletedTask;
        }
    }

    public record InputImapMail(string Inbox, string EmailFrom, string Subject, string Body, DateTime Date);
}