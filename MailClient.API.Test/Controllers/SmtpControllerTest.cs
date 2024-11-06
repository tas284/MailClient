using MailClient.Application.Controllers;
using MailClient.Application.InputModel;
using MailClient.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace MailClient.API.Test.Controllers
{
    public class SmtpControllerTest
    {
        [Fact]
        public void ShouldSendEmail()
        {
            SendEmailInputModel input = new()
            {
                User = "john@doe.com",
                Password = "john@doe.com",
                FromEmail = "john@doe.com",
                FromName = "john@doe.com",
                ToEmail = "john@doe.com",
                ToName = "john@doe.com",
                SmtpAddress = "john@doe.com",
                SmtpPort = 587,
                Subject = "john@doe.com",
                Body = "john@doe.com",
                BodyHtml = "john@doe.com"
            };

            Mock<IEmailSmtpService> _serviceMock = new Mock<IEmailSmtpService>();
            _serviceMock.Setup(_ => _.Send(input)).Returns("Email sent sucessfully");

            SmtpController smtpController = new SmtpController(_serviceMock.Object);

            OkObjectResult result = (OkObjectResult)smtpController.SendEmail(input);
            Assert.NotNull(result);
            Assert.Equal(200, result.StatusCode);
            Assert.Equal(result.Value, "Email sent sucessfully");
        }

        [Fact]
        public void ShouldNotSendEmail()
        {
            SendEmailInputModel input = new()
            {
                User = null,
                Password = null,
                FromEmail = null,
                FromName = null,
                ToEmail = null,
                ToName = null,
                SmtpAddress = null,
                SmtpPort = 587,
                Subject = null,
                Body = null,
                BodyHtml = null,
            };

            Mock<IEmailSmtpService> _serviceMock = new Mock<IEmailSmtpService>();
            _serviceMock.Setup(_ => _.Send(input)).Throws(new Exception("Invalid parameters to send email!"));
            SmtpController smtpController = new SmtpController(_serviceMock.Object);

            BadRequestObjectResult result = (BadRequestObjectResult) smtpController.SendEmail(input);
            Assert.NotNull(result);
            Assert.Equal(400, result.StatusCode);
            Assert.Equal("Invalid parameters to send email!", result.Value);
        }
    }
}
