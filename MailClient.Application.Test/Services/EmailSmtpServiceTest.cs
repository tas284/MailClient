using MailClient.Application.InputModel;
using MailClient.Application.Services;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using MimeKit;
using MongoDB.Driver;
using Moq;

namespace MailClient.Application.Test.Services
{
    public class EmailSmtpServiceTest
    {
        private readonly Mock<ILogger<EmailSmtpService>> _mockLogger;
        private readonly Mock<ISmtpClient> _mockSmtpClient;
        private readonly EmailSmtpService _emailSmtpService;

        public EmailSmtpServiceTest()
        {
            _mockLogger = new Mock<ILogger<EmailSmtpService>>();
            _mockSmtpClient = new Mock<ISmtpClient>();
            _emailSmtpService = new EmailSmtpService(_mockLogger.Object, _mockSmtpClient.Object);
        }

        [Fact]
        public void Send_ShouldReturnSuccess_WhenEmailSentSuccessfully()
        {
            SendEmailInputModel input = new SendEmailInputModel
            {
                SmtpAddress = "smtp.example.com",
                SmtpPort = 587,
                User = "user@example.com",
                Password = "password",
                FromName = "Sender Name",
                FromEmail = "sender@example.com",
                ToName = "Receiver Name",
                ToEmail = "receiver@example.com",
                Subject = "Test Email",
                Body = "This is a test email",
                BodyHtml = "<h1>This is a test email</h1>"
            };

            _mockSmtpClient.Setup(client => client.Connect(input.SmtpAddress, input.SmtpPort, SecureSocketOptions.Auto, new CancellationToken())).Verifiable();
            _mockSmtpClient.Setup(client => client.Authenticate(It.IsAny<string>(), It.IsAny<string>(), new CancellationToken())).Verifiable();
            _mockSmtpClient.Setup(client => client.Send(It.IsAny<MimeMessage>(), new CancellationToken(), null)).Returns("Ok");
            _mockSmtpClient.Setup(client => client.Disconnect(It.IsAny<bool>(), new CancellationToken())).Verifiable();

            string result = _emailSmtpService.Send(input);

            Assert.Contains("Email send succesfully to", result);
            _mockSmtpClient.Verify(client => client.Connect(input.SmtpAddress, input.SmtpPort, SecureSocketOptions.Auto, new CancellationToken()), Times.Once);
            _mockSmtpClient.Verify(client => client.Authenticate(It.IsAny<string>(), It.IsAny<string>(), new CancellationToken()), Times.Once);
            _mockSmtpClient.Verify(client => client.Disconnect(It.IsAny<bool>(), new CancellationToken()), Times.Once);
        }
    }
}
