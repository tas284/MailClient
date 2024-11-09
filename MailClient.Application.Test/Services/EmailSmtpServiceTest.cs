using MailClient.Application.InputModel;
using MailClient.Application.Services;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using MimeKit;
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

        [Fact(DisplayName = "Send Should Return Success When Email Sent Successfully")]
        public void Send_ShouldReturnSuccess_WhenEmailSentSuccessfully()
        {
            SendEmailInputModel input = GetSendEmailInputModel();

            _mockSmtpClient.Setup(client => client.Connect(input.SmtpAddress, input.SmtpPort, SecureSocketOptions.Auto, new CancellationToken())).Verifiable();
            _mockSmtpClient.Setup(client => client.Authenticate(input.User, input.Password, new CancellationToken())).Verifiable();
            _mockSmtpClient.Setup(client => client.Send(It.IsAny<MimeMessage>(), new CancellationToken(), null)).Returns("Ok");
            _mockSmtpClient.Setup(client => client.Disconnect(It.IsAny<bool>(), new CancellationToken())).Verifiable();

            string result = _emailSmtpService.Send(input);

            Assert.Contains("Email send succesfully to", result);
            _mockSmtpClient.Verify(client => client.Connect(input.SmtpAddress, input.SmtpPort, SecureSocketOptions.Auto, new CancellationToken()), Times.Once);
            _mockSmtpClient.Verify(client => client.Authenticate(input.User, input.Password, new CancellationToken()), Times.Once);
            _mockSmtpClient.Verify(client => client.Disconnect(It.IsAny<bool>(), new CancellationToken()), Times.Once);
        }

        [Fact(DisplayName = "Send Should Trows Exception When Email Sent With Invalid Smtp Address or Port invalid")]
        public void Send_ShouldTrowsException_WhenEmailSentWithInvalidSmtpAddressOrPortInvalid()
        {
            SendEmailInputModel input = new SendEmailInputModel
            {
                SmtpAddress = null,
                SmtpPort = 0
            };
            string expectecResult = "Error to send email: Invalid smtp address and port";

            _mockSmtpClient.Setup(client => client.Connect(input.SmtpAddress, input.SmtpPort, SecureSocketOptions.Auto, new CancellationToken())).Throws(new Exception(expectecResult));

            string result = _emailSmtpService.Send(input);

            Assert.Equal(expectecResult, result);
            Assert.NotNull(result);

        }

        [Fact(DisplayName = "Send Should Trows Exception When Email Sent With Invalid User or Password invalid")]
        public void Send_ShouldTrowsException_WhenEmailSentWithInvalidUserOrPassworedInvalid()
        {
            SendEmailInputModel input = new SendEmailInputModel
            {
                User = null,
                Password = null,
            };
            string expectecResult = "Error to send email: Invalid user or password";
            _mockSmtpClient.Setup(client => client.Connect(input.SmtpAddress, input.SmtpPort, SecureSocketOptions.Auto, new CancellationToken())).Verifiable();
            _mockSmtpClient.Setup(client => client.Authenticate(input.User, input.Password, new CancellationToken())).Throws(new Exception(expectecResult));

            string result = _emailSmtpService.Send(input);

            Assert.Equal(expectecResult, result);
            Assert.NotNull(result);
        }

        [Fact(DisplayName = "Send Should Trows Exception if FromEmail invalid for CreateMessage")]
        public void Send_ShouldTrowsException_WhenFromEmailInvalidForCreateMessage()
        {
            SendEmailInputModel input = GetSendEmailInputModel();
            input.FromEmail = input.FromName = null;
            string expectecResult = "Value cannot be null. (Parameter 'address')";

            _mockSmtpClient.Setup(client => client.Connect(input.SmtpAddress, input.SmtpPort, SecureSocketOptions.Auto, new CancellationToken())).Verifiable();
            _mockSmtpClient.Setup(client => client.Authenticate(input.User, input.Password, new CancellationToken())).Verifiable();

            string result = _emailSmtpService.Send(input);

            Assert.Equal(expectecResult, result);
            Assert.NotNull(result);
        }

        [Fact(DisplayName = "Send Should Trows Exception if ToEmail invalid for CreateMessage")]
        public void Send_ShouldTrowsException_WhenToEmailInvalidForCreateMessage()
        {
            SendEmailInputModel input = GetSendEmailInputModel();
            input.ToEmail = input.ToName = null;
            string expectecResult = "Value cannot be null. (Parameter 'address')";

            _mockSmtpClient.Setup(client => client.Connect(input.SmtpAddress, input.SmtpPort, SecureSocketOptions.Auto, new CancellationToken())).Verifiable();
            _mockSmtpClient.Setup(client => client.Authenticate(input.User, input.Password, new CancellationToken())).Verifiable();

            string result = _emailSmtpService.Send(input);

            Assert.Equal(expectecResult, result);
            Assert.NotNull(result);
        }

        [Fact(DisplayName = "Send Should Trows Exception if Subject invalid for CreateMessage")]
        public void Send_ShouldTrowsException_WhenSubjectInvalidForCreateMessage()
        {
            SendEmailInputModel input = GetSendEmailInputModel();
            input.Subject = null;
            string expectecResult = "Value cannot be null. (Parameter 'value')";

            _mockSmtpClient.Setup(client => client.Connect(input.SmtpAddress, input.SmtpPort, SecureSocketOptions.Auto, new CancellationToken())).Verifiable();
            _mockSmtpClient.Setup(client => client.Authenticate(input.User, input.Password, new CancellationToken())).Verifiable();

            string result = _emailSmtpService.Send(input);

            Assert.Equal(expectecResult, result);
            Assert.NotNull(result);
        }

        [Fact(DisplayName = "Send Should Trows Exception if Body invalid for CreateBody")]
        public void Send_ShouldTrowsException_WhenBodyInvalidForCreateBody()
        {
            SendEmailInputModel input = GetSendEmailInputModel();
            input.Body = input.BodyHtml = null;
            string expectecResult = "Value cannot be null. (Parameter 'text')";

            _mockSmtpClient.Setup(client => client.Connect(input.SmtpAddress, input.SmtpPort, SecureSocketOptions.Auto, new CancellationToken())).Verifiable();
            _mockSmtpClient.Setup(client => client.Authenticate(input.User, input.Password, new CancellationToken())).Verifiable();

            string result = _emailSmtpService.Send(input);

            Assert.Equal(expectecResult, result);
            Assert.NotNull(result);
        }

        #region Helpers
        private SendEmailInputModel GetSendEmailInputModel()
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

            return input;
        }
        #endregion
    }
}
