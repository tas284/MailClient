using MailClient.Application.Controllers;
using MailClient.Application.InputModel;
using MailClient.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace MailClient.API.Test.Controllers
{
    public class SmtpControllerTest
    {
        private readonly Mock<IEmailSmtpService> _mockService;
        private readonly SmtpController _smptController;

        public SmtpControllerTest()
        {
            _mockService = new Mock<IEmailSmtpService>();
            _smptController = new SmtpController(_mockService.Object);
        }

        [Fact]
        public void SendEmail_ReturnsOkResult_WhebnServiceSucceds()
        {
            SendEmailInputModel input = new()
            {
                ToEmail = "john@doe.com",
                Subject = "John Doe",
                Body = "Hello from XUnit Test",
            };
            string expectedResult = "Email sent successfully";

            _mockService.Setup(service => service.Send(input)).Returns(expectedResult);

            var result = _smptController.SendEmail(input);
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(expectedResult, okResult.Value);
        }

        [Fact]
        public void SendEmail_ReturnsBadRequest_WhenServiceThrowsException()
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
            string expectedMessage = "An error ocurred while send the email";

            _mockService.Setup(service => service.Send(input)).Throws(new Exception(expectedMessage));

            var result = _smptController.SendEmail(input);

            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(expectedMessage, badRequestResult.Value);
        }
    }
}
