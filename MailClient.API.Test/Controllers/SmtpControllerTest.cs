using MailClient.Application.Controllers;
using MailClient.Application.InputModel;
using MailClient.Application.Interfaces;
using Microsoft.AspNetCore.Http;
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

<<<<<<< Updated upstream
        [Fact]
=======
        [Fact(DisplayName = "Send Email should returns ok result when service succeds")]
>>>>>>> Stashed changes
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

            IActionResult result = _smptController.SendEmail(input);

            var okObjectResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, okObjectResult.StatusCode);
            Assert.NotNull(okObjectResult);
            Assert.Equal(expectedResult, okObjectResult.Value);
        }

<<<<<<< Updated upstream
        [Fact]
=======
        [Fact(DisplayName = "Send Email should returns bad request when service throws exception")]
>>>>>>> Stashed changes
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

            IActionResult result = _smptController.SendEmail(input);

            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(400, badRequestResult.StatusCode);
            Assert.NotNull(badRequestResult);
            Assert.Equal(expectedMessage, badRequestResult.Value);
        }
    }
}
