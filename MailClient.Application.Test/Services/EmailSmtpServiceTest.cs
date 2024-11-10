﻿using MailClient.Application.InputModel;
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

        [Fact(DisplayName = "Send Should Trows ArgumentException When Email Sent With Invalid Smtp Address or Port invalid")]
        public void Send_ShouldTrowsArgumentException_WhenEmailSentWithInvalidSmtpAddressOrPortInvalid()
        {
            SendEmailInputModel input = GetSendEmailInputModel();
            input.SmtpAddress = null;
            input.SmtpPort = 0;
            string expectecResult = "Invalid SMTP:PORT Adrress";

            ArgumentException exception = Assert.Throws<ArgumentException>(() => _emailSmtpService.Send(input));

            Assert.Equal(expectecResult, exception.Message);
            Assert.NotNull(exception);
        }

        [Fact(DisplayName = "Send Should Trows ArgumentException When Email Sent With Invalid User or Password invalid")]
        public void Send_ShouldTrowsArgumentException_WhenEmailSentWithInvalidUserOrPassworedInvalid()
        {
            SendEmailInputModel input = GetSendEmailInputModel();
            input.User = input.Password = null;
            string expectecResult = "Invalid user and password";

            ArgumentException exception = Assert.Throws<ArgumentException>(() => _emailSmtpService.Send(input));

            Assert.Equal(expectecResult, exception.Message);
            Assert.NotNull(exception);
        }

        [Fact(DisplayName = "Send Should Trows ArgumentException if FromEmail invalid for CreateMessage")]
        public void Send_ShouldTrowsArgumentException_WhenFromEmailInvalidForCreateMessage()
        {
            SendEmailInputModel input = GetSendEmailInputModel();
            input.FromEmail = input.FromName = null;
            string expectecResult = "Invalid sender email address";

            ArgumentException exception = Assert.Throws<ArgumentException>(() => _emailSmtpService.Send(input));

            Assert.Equal(expectecResult, exception.Message);
            Assert.NotNull(exception);
        }

        [Fact(DisplayName = "Send Should Trows ArgumentException if ToEmail invalid for CreateMessage")]
        public void Send_ShouldTrowsArgumentException_WhenToEmailInvalidForCreateMessage()
        {
            SendEmailInputModel input = GetSendEmailInputModel();
            input.ToEmail = input.ToName = null;
            string expectecResult = "Invalid recipient email address";

            ArgumentException exception = Assert.Throws<ArgumentException>(() => _emailSmtpService.Send(input));

            Assert.Equal(expectecResult, exception.Message);
            Assert.NotNull(exception);
        }

        [Fact(DisplayName = "Send Should Trows ArgumentException if Subject invalid for CreateMessage")]
        public void Send_ShouldTrowsArgumentException_WhenSubjectInvalidForCreateMessage()
        {
            SendEmailInputModel input = GetSendEmailInputModel();
            input.Subject = null;
            string expectecResult = "Subject cannot be empty";

            ArgumentException exception = Assert.Throws<ArgumentException>(() => _emailSmtpService.Send(input));

            Assert.Equal(expectecResult, exception.Message);
            Assert.NotNull(exception);
        }

        [Fact(DisplayName = "Send Should Trows ArgumentException if Body invalid for CreateBody")]
        public void Send_ShouldTrowsArgumentException_WhenBodyInvalidForCreateBody()
        {
            SendEmailInputModel input = GetSendEmailInputModel();
            input.Body = input.BodyHtml = null;
            string expectecResult = "At least one body (text or HTML) must be provided";

            ArgumentException exception = Assert.Throws<ArgumentException>(() => _emailSmtpService.Send(input));

            Assert.Equal(expectecResult, exception.Message);
            Assert.NotNull(exception);
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
