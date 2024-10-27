using MailClient.API.Configuration;
using MailClient.API.InputModel;
using MailClient.API.Interfaces;
using MailClient.API.Model;
using MailKit;
using MailKit.Net.Imap;
using MailKit.Search;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MailClient.API.Services
{
    public class EmailImapService : IEmailImapService
    {
        private readonly RabbitMqConfiguration _configuration;
        private readonly ConnectionFactory _factory;
        private ILogger<EmailImapService> _logger;

        public EmailImapService(IOptions<RabbitMqConfiguration> configuration, ILogger<EmailImapService> logger)
        {
            _configuration = configuration.Value;
            _factory = new ConnectionFactory { HostName = _configuration.Host };
            _logger = logger;
        }

        public async Task<string> SyncMessages(SyncEmailImapInputModel input)
        {
            var total = 0;
            using (var client = new ImapClient())
            {
                client.Connect(input.ImapAddress, input.ImapPort, SecureSocketOptions.Auto);

                try
                {
                    client.Authenticate(input.User, input.Password);
                    _logger.LogInformation($"Email authenticated on: {input.ImapAddress}:{input.ImapPort}");

                    var inbox = client.Inbox;
                    var uids = GetUids(input.DateSync, inbox);
                    _logger.LogInformation($"{uids.Count} messages find.");

                    foreach (var uid in uids)
                    {
                        var message = inbox.GetMessage(uid);
                        await Task.Run(() => Handle(input.User, message.From.Mailboxes.FirstOrDefault()!.Address, message.Subject, message.HtmlBody, message.Date.LocalDateTime));
                        total++;
                    }
                }
                catch (Exception ex)
                {
                    var error = $"Erro to sync messages: {ex.Message}";
                    _logger.LogError($"Erro: {error}.");
                    return error;
                }

                client.Disconnect(true);
            }
            string result = $"{total} messages sync!";
            _logger.LogInformation(result);
            return result;
        }

        private IList<UniqueId> GetUids(DateTime initial, IMailFolder folder)
        {
            folder.Open(FolderAccess.ReadOnly);

            var query = SearchQuery.DeliveredAfter(initial);
            var uids = folder.Search(query);

            return uids;
        }

        private async void Handle(string inbox, string emailFrom, string subject, string body, DateTime date)
        {
            await Publish(new InputImapMail(inbox, emailFrom, subject, body, date));
        }

        private async Task Publish(InputImapMail message)
        {
            using var connection = _factory.CreateConnection();
            using var channel = connection.CreateModel();

            channel.QueueDeclare(
                queue: _configuration.QueueMail,
                durable: false,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            string stringifiedMessage = JsonConvert.SerializeObject(message);
            byte[] bytesMessage = Encoding.UTF8.GetBytes(stringifiedMessage);

            channel.BasicPublish(
                exchange: "",
                routingKey: _configuration.QueueMail,
                basicProperties: null,
                body: bytesMessage);

            _logger.LogInformation($"Message published on broker: {message.Subject}.");

            await Task.CompletedTask;
        }
    }
}