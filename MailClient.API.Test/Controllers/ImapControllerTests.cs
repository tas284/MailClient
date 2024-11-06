using MailClient.Application.Controllers;
using MailClient.Application.InputModel;
using MailClient.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace MailClient.API.Test.Controllers
{
    public class ImapControllerTests
    {
        private readonly Mock<IEmailImapService> _mockService;
        private readonly ImapController _imapController;

        public ImapControllerTests()
        {
            _mockService = new Mock<IEmailImapService>();
            _imapController = new ImapController(_mockService.Object);
        }

        [Fact]
        public void SyncMessages_ReturnsOkResult_WhenServiceSucceds()
        {
            var input = new SyncEmailImapInputModel
            {
                User = "john@doe.com",
                Password = "john@doe.com"
            };
            var expectedResult = "Sync completed succesfully";

            _mockService.Setup(service => service.SyncMessages(input)).Returns(expectedResult);

            var result = _imapController.SyncMessages(input);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(expectedResult, okResult.Value);
        }

        [Fact]
        public void SyncMessages_ReturnsBadRequest_WhenServiceThrowsException()
        {
            var input = new SyncEmailImapInputModel
            {
                User = "john@doe.com",
                Password = "john@doe.com"
            };
            var expectedResult = "An error ocurred while syncing messages";

            _mockService.Setup(service => service.SyncMessages(input)).Throws(new Exception(expectedResult));

            var result = _imapController.SyncMessages(input);

            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(expectedResult, badRequestResult.Value);
        }
    }
}
