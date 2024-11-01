using MailClient.Application.Configuration;
using MailClient.Application.InputModel;
using MailClient.Application.Interfaces;
using MailClient.Application.Model;
using MailKit;
using MailKit.Net.Imap;
using MailKit.Search;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System.Text;

namespace MailClient.Application.Services
{
    public class EmailImapService : IEmailImapService
    {
        private readonly RabbitMqConfiguration _configuration;
        private readonly ConnectionFactory _factory;
        private ILogger<EmailImapService> _logger;

        public EmailImapService(IOptions<RabbitMqConfiguration> configuration, ILogger<EmailImapService> logger)
        {
            _configuration = configuration.Value;
            _factory = new ConnectionFactory { HostName = _configuration.Host, RequestedHeartbeat = TimeSpan.FromSeconds(30), SocketReadTimeout = TimeSpan.FromSeconds(30) };
            _logger = logger;
        }

        public string SyncMessages(SyncEmailImapInputModel input)
        {
            List<SyncEmailImapInputModel> listSyncEmailImapInputModel = GetRangeSyncEmailImapInputModel(input);

            var total = 0;
            var parallelOptions = new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount };

            Parallel.ForEach(listSyncEmailImapInputModel, parallelOptions, input =>
            {
                using (var client = new ImapClient())
                {
                    client.Connect(input.ImapAddress, input.ImapPort, SecureSocketOptions.Auto);
                    _logger.LogInformation($"Client Connected.");

                    try
                    {
                        client.Authenticate(input.User, input.Password);
                        _logger.LogInformation($"Email authenticated on: {input.ImapAddress}:{input.ImapPort}");

                        var inbox = client.Inbox;
                        var uids = GetUids(input.StartDate, input.EndDate, inbox);
                        _logger.LogInformation($"{uids.Count} messages find.");

                        foreach (var uid in uids)
                        {
                            var message = inbox.GetMessage(uid);
                            Handle(input.User, message.From.Mailboxes.FirstOrDefault()!.Address, message.Subject, message.HtmlBody, message.Date.LocalDateTime);
                            total++;
                        }
                    }
                    catch (Exception ex)
                    {
                        var error = $"Erro to sync messages: {ex.Message}";
                        _logger.LogError($"Erro: {error}.");
                    }

                    client.Disconnect(true);
                    _logger.LogInformation($"Client disconnected.");
                }
            });

            string result = $"{total} messages sync!";
            _logger.LogInformation(result);
            return result;
        }

        private List<SyncEmailImapInputModel> GetRangeSyncEmailImapInputModel(SyncEmailImapInputModel input)
        {
            List<SyncEmailImapInputModel> listSyncEmailImapInputModel = new();
            double days = GetDays(input.StartDate);

            input.EndDate = input.StartDate.AddDays(days);

            while (input.EndDate < DateTime.Now)
            {
                var item = new SyncEmailImapInputModel
                {
                    User = input.User,
                    Password = input.Password,
                    ImapAddress = input.ImapAddress,
                    ImapPort = input.ImapPort,
                    StartDate = input.StartDate,
                    EndDate = input.EndDate
                };

                listSyncEmailImapInputModel.Add(item);

                input.StartDate = input.EndDate;
                input.EndDate = input.StartDate.AddDays(days);
            }

            return listSyncEmailImapInputModel;
        }

        private double GetDays(DateTime date)
        {
            double days = 3;

            DateTime first = DateTime.Now.AddDays(-30);
            DateTime second = DateTime.Now.AddDays(-60);
            DateTime third = DateTime.Now.AddDays(-120);
            DateTime fourth = DateTime.Now.AddDays(-180);
            DateTime fifth = DateTime.Now.AddDays(-240);

            if (date < first && date > second)
            {
                days = 5;
            }
            else if (date < second && date > third)
            {
                days = 10;
            }
            else if (date < third && date > fourth)
            {
                days = 15;
            }
            else if (date < fourth && date > fifth)
            {
                days = 20;
            }
            else if (date < fifth)
            {
                days = 30;
            }

            return days;
        }

        private IList<UniqueId> GetUids(DateTime startDate, DateTime endDate, IMailFolder folder)
        {
            folder.Open(FolderAccess.ReadOnly);

            var deliveryAfter = SearchQuery.DeliveredAfter(startDate);
            var deliveryBefore = SearchQuery.DeliveredBefore(endDate);
            var uids = folder.Search(SearchQuery.And(deliveryAfter, deliveryBefore));

            return uids;
        }

        private void Handle(string inbox, string emailFrom, string subject, string body, DateTime date)
        {
            Publish(new InputImapMail(inbox, emailFrom, subject, body, date));
        }

        private void Publish(InputImapMail message)
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
        }
    }
}