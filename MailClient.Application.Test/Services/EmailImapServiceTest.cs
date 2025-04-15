using MailClient.Application.InputModel;
using MailClient.Application.Services;
using MailClient.Domain.Repositories;
using MailClient.Infrastructure.Interfaces;
using MailClient.Infrastructure.Model;
using MailKit;
using MailKit.Net.Imap;
using MailKit.Search;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using MimeKit;
using Moq;

namespace MailClient.Application.Test.Services
{
    public class EmailImapServiceTest
    {
        private readonly Mock<IPublisher> _mockPublisher;
        private readonly Mock<IRepositoryEmail> _mockRepository;
        private readonly Mock<IServiceProvider> _mockServiceProvider;
        private readonly Mock<ILogger<EmailImapService>> _mockLogger;
        private readonly Mock<IImapClient> _mockImapClient;
        private readonly Mock<IMailFolder> _mockMailFolder;
        private readonly EmailImapService _emailImapServiceSut;

        public EmailImapServiceTest()
        {
            _mockPublisher = new Mock<IPublisher>();
            _mockRepository = new Mock<IRepositoryEmail>();
            _mockServiceProvider = new Mock<IServiceProvider>();
            _mockLogger = new Mock<ILogger<EmailImapService>>();
            _mockImapClient = new Mock<IImapClient>();

            _mockServiceProvider
                .Setup(sp => sp.GetService(typeof(IImapClient)))
                .Returns(_mockImapClient.Object);

            _emailImapServiceSut = new EmailImapService(
                _mockPublisher.Object,
                _mockRepository.Object,
                _mockServiceProvider.Object,
                _mockLogger.Object
            );

            _mockMailFolder = new Mock<IMailFolder>();
        }

        [Fact(DisplayName = "Sync Messages should return success when emails are imported with success")]
        public void SyncMessages_ShouldReturnSuccess_WhenEmailAreImportedWithSuccess()
        {
            int total = 10;
            string expectedResult = $"{total} messages sync";
            ImapInputModel input = GetSyncEmailImapInputModel();

            var cancelattionToken = new CancellationToken();
            List<UniqueId> mockUids = Enumerable.Range(1, total).Select(i => new UniqueId((uint)i)).ToList();

            _mockImapClient.Setup(c => c.Connect(input.ImapAddress, input.ImapPort, It.IsAny<SecureSocketOptions>(), cancelattionToken)).Verifiable();
            _mockImapClient.Setup(c => c.Authenticate(input.User, input.Password, cancelattionToken)).Verifiable();
            _mockImapClient.Setup(c => c.Inbox).Returns(_mockMailFolder.Object);
            _mockImapClient.Setup(c => c.Inbox.Search(It.IsAny<SearchQuery>(), cancelattionToken)).Returns(mockUids);
            _mockMailFolder.Setup(c => c.GetMessage(It.IsAny<UniqueId>(), cancelattionToken, It.IsAny<ITransferProgress>())).Returns(GetMimeMessage());

            string actualResult = _emailImapServiceSut.SyncMessages(input);

            Assert.NotNull(actualResult);
            Assert.Contains(expectedResult, actualResult);
            _mockImapClient.Verify(c => c.Connect(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<SecureSocketOptions>(), cancelattionToken), Times.Once);
            _mockImapClient.Verify(c => c.Authenticate(It.IsAny<string>(), It.IsAny<string>(), cancelattionToken), Times.Once);
            _mockPublisher.Verify(p => p.Publish(It.IsAny<ImapMailMessage>()), Times.Exactly(10));
        }

        [Fact(DisplayName = "Sync Messages should throws argument exception when SyncEmailImapInputModel is invalid")]
        public void SyncMessages_ShouldThrowsArugmentException_WhenSyncEmailImapInputModelIsInvalid()
        {
            string expectedResult = "Invalid IMAP:PORT Adrress";
            ImapInputModel syncEmailImapInputModel = GetSyncEmailImapInputModel();
            syncEmailImapInputModel.ImapAddress = string.Empty;
            syncEmailImapInputModel.ImapPort = 0;

            var actualResult = Assert.Throws<ArgumentException>(() => _emailImapServiceSut.SyncMessages(syncEmailImapInputModel));

            Assert.NotNull(actualResult);
            Assert.Equal(expectedResult, actualResult.Message);
        }

        private ImapInputModel GetSyncEmailImapInputModel()
        {
            return new ImapInputModel
            {
                User = "john@doe.com",
                Password = "password",
                ImapAddress = "imap.email.com",
                ImapPort = 993,
                StartDate = DateTime.Now.AddDays(-1),
            };
        }

        private MimeMessage GetMimeMessage()
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("Test xUnit", "john@doe.com"));
            message.To.AddRange([new MailboxAddress("Test xUnit", "john@doe.com")]);
            message.Subject = "Subject";

            return message;
        }
    }
}
