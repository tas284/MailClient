using MailClient.Application.InputModel;
using MailClient.Application.Interfaces;
using MailClient.Application.Specification;
using MailClient.Infrastructure.Interfaces;
using MailClient.Infrastructure.Model;
using MailKit;
using MailKit.Net.Imap;
using MailKit.Search;
using MailKit.Security;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MimeKit;
using System.Diagnostics;

namespace MailClient.Application.Services
{
    public class EmailImapService : IEmailImapService
    {
        private readonly IPublisher _publisher;
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<EmailImapService> _logger;
        private readonly ISpecification<SyncEmailImapInputModel> _spec;

        public EmailImapService(IPublisher publisher, IServiceProvider serviceProvider, ILogger<EmailImapService> logger)
        {
            _publisher = publisher;
            _serviceProvider = serviceProvider;
            _logger = logger;
            _spec = new IsValidSyncEmailImapInputModelSpec();
        }

        public string SyncMessages(SyncEmailImapInputModel input)
        {
            if (!_spec.IsSatisfiedBy(input)) throw new ArgumentException(input.Validations);

            var rangeSyncEmailImapInputModel = GetRangeSyncEmailImapInputModel(input);

            int total = 0;
            int skip = 0;
            int take = 12;
            int countMessages = 0;
            ParallelOptions options = new ParallelOptions { MaxDegreeOfParallelism = take };

            Stopwatch sw = new();
            sw.Start();

            var range = rangeSyncEmailImapInputModel.SkipLast(skip).TakeLast(take).ToList();
            do
            {
                Parallel.ForEach(range, options, input =>
                {
                    using var client = _serviceProvider.GetRequiredService<IImapClient>();
                    try
                    {
                        client.Connect(input.ImapAddress, input.ImapPort, SecureSocketOptions.Auto);
                        _logger.LogInformation($"Client Connected.");

                        client.Authenticate(input.User, input.Password);
                        _logger.LogInformation($"Email authenticated on: {input.ImapAddress}:{input.ImapPort}");

                        var inbox = client.Inbox;
                        var uids = GetUids(input.StartDate, input.EndDate, inbox);
                        _logger.LogInformation($"{uids.Count} messages find.");
                        countMessages = uids.Count;

                        foreach (var uid in uids)
                        {
                            var message = inbox.GetMessage(uid);
                            _publisher.Publish(ImapMailMessage.Create(message));
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
                });

                skip += range.Count;
                range = rangeSyncEmailImapInputModel.Skip(skip).Take(take).ToList();

            } while (range.Count > 0 && countMessages > 0);

            sw.Stop();

            var result = $"{total} messages sync";
            _logger.LogInformation($"{result} in {sw.ElapsedMilliseconds / 1000} seconds!");
            return result;
        }

        private List<SyncEmailImapInputModel> GetRangeSyncEmailImapInputModel(SyncEmailImapInputModel input)
        {
            var listSyncEmailImapInputModel = new List<SyncEmailImapInputModel>();
            var days = GetDays(input.StartDate);

            input.EndDate = input.StartDate.AddDays(days);

            while (input.EndDate < DateTime.Now.AddDays(1))
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

        private int GetDays(DateTime date)
        {
            var days = 2;

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
            var deliveryAfter = SearchQuery.DeliveredAfter(startDate);
            var deliveryBefore = SearchQuery.DeliveredBefore(endDate);
            return folder.Search(SearchQuery.And(deliveryAfter, deliveryBefore));
        }
    }
}