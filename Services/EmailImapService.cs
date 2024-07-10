using MailClient.Configuration;
using MailClient.InputModel;
using MailClient.Interfaces;
using MailKit;
using MailKit.Net.Imap;
using MailKit.Search;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System.Text;

namespace MailClient.Services
{
    public class EmailImapService : IEmailImapService
    {
        private readonly RabbitMqConfiguration _configuration;
        private readonly ConnectionFactory _factory;

        public EmailImapService(IOptions<RabbitMqConfiguration> configuration)
        {
            _configuration = configuration.Value;
            _factory = new ConnectionFactory { HostName = _configuration.Host };
        }

        public Task<string> SyncMessages(SyncEmailImapInputModel input)
        {
            var total = 0;
            using (var client = new ImapClient())
            {
                client.Connect(input.ImapAddress, input.ImapPort, true);

                try
                {
                    client.Authenticate(input.User, input.Password);
                    var inbox = client.Inbox;

                    foreach (var uid in GetUids(input.DateSync, inbox))
                    {
                        var message = inbox.GetMessage(uid);
                        Handle(input.User, message.From.Mailboxes.FirstOrDefault()!.Address, message.Subject, message.HtmlBody, message.Date.LocalDateTime);
                        total++;
                    }
                }
                catch (Exception ex)
                {
                    var error = $"Erro to sync messages: {ex.Message}";
                    return Task.FromResult(error);
                }

                client.Disconnect(true);
            }
            return Task.FromResult($"{total} messages sync!");
        }

        private static IList<UniqueId> GetUids(DateTime initial, IMailFolder folder)
        {
            folder.Open(FolderAccess.ReadOnly);

            var query = SearchQuery.DeliveredAfter(initial);
            var uids = folder.Search(query);

            return uids;
        }

        private void Handle(string inbox, string emailFrom, string subject, string body, DateTime date)
        {
            Publish(new InputImapMail(inbox, emailFrom, subject, body, date));
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