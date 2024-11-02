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
using MimeKit;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System.Diagnostics;
using System.Text;

namespace MailClient.Application.Services
{
    public class EmailImapService : IEmailImapService
    {
        private readonly RabbitMqConfiguration _configuration;
        private readonly ConnectionFactory _factory;
        private readonly ILogger<EmailImapService> _logger;

        public EmailImapService(IOptions<RabbitMqConfiguration> configuration, ILogger<EmailImapService> logger)
        {
            _configuration = configuration.Value;
            _factory = new ConnectionFactory { HostName = _configuration.Host, RequestedHeartbeat = TimeSpan.FromSeconds(30), SocketReadTimeout = TimeSpan.FromSeconds(30) };
            _logger = logger;
        }

        public string SyncMessages(SyncEmailImapInputModel input)
        {
            List<SyncEmailImapInputModel> rangeSyncEmailImapInputModel = GetRangeSyncEmailImapInputModel(input);

            int total = 0;
            int skip = 0;
            int count = Environment.ProcessorCount;
            ParallelOptions options = new ParallelOptions { MaxDegreeOfParallelism = count };

            Stopwatch sw = new();
            sw.Start();

            List<SyncEmailImapInputModel> range = rangeSyncEmailImapInputModel.SkipLast(skip).TakeLast(count).ToList();
            while (range.Count > 0)
            {
                Parallel.ForEach(range, options, input =>
                {
                    using (var client = new ImapClient())
                    {
                        client.Connect(input.ImapAddress, input.ImapPort, SecureSocketOptions.Auto);
                        _logger.LogInformation($"Client Connected.");

                        try
                        {
                            client.Authenticate(input.User, input.Password);
                            _logger.LogInformation($"Email authenticated on: {input.ImapAddress}:{input.ImapPort}");

                            IMailFolder inbox = client.Inbox;
                            IList<UniqueId> uids = GetUids(input.StartDate, input.EndDate, inbox);
                            _logger.LogInformation($"{uids.Count} messages find.");

                            foreach (var uid in uids)
                            {
                                MimeMessage message = inbox.GetMessage(uid);
                                Publish(ImapMailMessage.Create(message));
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

                skip += range.Count;
                range = rangeSyncEmailImapInputModel.Skip(skip).Take(count).ToList();
            }

            sw.Stop();

            string result = $"{total} messages sync";
            _logger.LogInformation($"{result} in {sw.ElapsedMilliseconds / 1000} seconds!");
            return result;
        }

        private List<SyncEmailImapInputModel> GetRangeSyncEmailImapInputModel(SyncEmailImapInputModel input)
        {
            List<SyncEmailImapInputModel> listSyncEmailImapInputModel = new();
            double days = GetDays(input.StartDate);

            input.EndDate = input.StartDate.AddDays(days);

            while (input.EndDate < DateTime.Now.AddDays(1))
            {
                SyncEmailImapInputModel item = new SyncEmailImapInputModel
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
            double days = 2;

            DateTime first = DateTime.Now.AddDays(-60);
            DateTime second = DateTime.Now.AddDays(-120);
            DateTime third = DateTime.Now.AddDays(-240);
            DateTime fourth = DateTime.Now.AddDays(-360);

            if (date < first && date > second)
            {
                days = 3;
            }
            else if (date < second && date > third)
            {
                days = 6;
            }
            else if (date < third && date > fourth)
            {
                days = 12;
            }
            else if (date < fourth)
            {
                days = 30;
            }

            return days;
        }

        private IList<UniqueId> GetUids(DateTime startDate, DateTime endDate, IMailFolder folder)
        {
            folder.Open(FolderAccess.ReadOnly);

            DateSearchQuery deliveryAfter = SearchQuery.DeliveredAfter(startDate);
            DateSearchQuery deliveryBefore = SearchQuery.DeliveredBefore(endDate);
            return folder.Search(SearchQuery.And(deliveryAfter, deliveryBefore));
        }

        private void Publish(ImapMailMessage message)
        {
            using IConnection connection = _factory.CreateConnection();
            using IModel channel = connection.CreateModel();

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