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
        private readonly SmtpController _smptControllerSut;

        public SmtpControllerTest()
        {
            _mockService = new Mock<IEmailSmtpService>();
            _smptControllerSut = new SmtpController(_mockService.Object);
        }

        [Fact(DisplayName = "Send Email should returns ok result when service succeds")]
        public void SendEmail_ReturnsOkResult_WhenServiceSucceds()
        {
            SendEmailInputModel input = new()
            {
                ToEmail = "john@doe.com",
                Subject = "John Doe",
                Body = "Hello from XUnit Test",
            };
            string expectedResult = "Email sent successfully";
            _mockService.Setup(service => service.Send(input)).Returns(expectedResult);

            IActionResult result = _smptControllerSut.SendEmail(input);

            var okObjectResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, okObjectResult.StatusCode);
            Assert.NotNull(okObjectResult);
            Assert.Equal(expectedResult, okObjectResult.Value);
        }

        [Fact(DisplayName = "Send Email should returns bad request when service throws exception")]
        public void SendEmail_ReturnsBadRequest_WhenServiceThrowsException()
        {
            SendEmailInputModel input = new SendEmailInputModel()
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

            IActionResult result = _smptControllerSut.SendEmail(input);

            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(400, badRequestResult.StatusCode);
            Assert.NotNull(badRequestResult);
            Assert.Equal(expectedMessage, badRequestResult.Value);
        }
    }
}
