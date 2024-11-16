using MailClient.Application.Controllers;
using MailClient.Application.InputModel;
using MailClient.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace MailClient.API.Test.Controllers
{
    public class ImapControllerTest
    {
        private readonly Mock<IEmailImapService> _mockService;
        private readonly ImapController _imapControllerSut;

        public ImapControllerTest()
        {
            _mockService = new Mock<IEmailImapService>();
            _imapControllerSut = new ImapController(_mockService.Object);
        }

        [Fact(DisplayName = "Sync Messages should returns ok result when service succeds")]
        public void SyncMessages_ReturnsOkResult_WhenServiceSucceds()
        {
            SyncEmailImapInputModel input = new SyncEmailImapInputModel
            {
                User = "john@doe.com",
                Password = "john@doe.com"
            };
            string expectedResult = "Sync completed succesfully";

            _mockService.Setup(service => service.SyncMessages(input)).Returns(expectedResult);

            IActionResult result = _imapControllerSut.SyncMessages(input);

            OkObjectResult okObjectResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, okObjectResult.StatusCode);
            Assert.NotNull(okObjectResult);
            Assert.Equal(expectedResult, okObjectResult.Value);
        }

        [Fact(DisplayName = "Sync Messages should returns bad request when service throws exception")]
        public void SyncMessages_ReturnsBadRequest_WhenServiceThrowsException()
        {
            SyncEmailImapInputModel input = new SyncEmailImapInputModel
            {
                User = "john@doe.com",
                Password = "john@doe.com"
            };
            string expectedResult = "An error ocurred while syncing messages";

            _mockService.Setup(service => service.SyncMessages(input)).Throws(new Exception(expectedResult));

            IActionResult result = _imapControllerSut.SyncMessages(input);

            BadRequestObjectResult badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(400, badRequestResult.StatusCode);
            Assert.NotNull(badRequestResult);
            Assert.Equal(expectedResult, badRequestResult.Value);
        }
    }
}
