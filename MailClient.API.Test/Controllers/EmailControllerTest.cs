using MailClient.Application.Controllers;
using MailClient.Application.DTO;
using MailClient.Application.Exceptions;
using MailClient.Application.Interfaces;
using MailClient.Application.Paginator;
using MailClient.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace MailClient.API.Test.Controllers
{
    public class EmailControllerTest
    {
        private readonly Mock<IEmailService> _mockService;
        private readonly EmailController _emailControllerSut;

        public EmailControllerTest()
        {
            _mockService = new Mock<IEmailService>();
            _emailControllerSut = new EmailController(_mockService.Object);
        }

        [Fact(DisplayName = "Get All Emails should return all emails according to the given skip and pageSize")]
        public async Task GetAllEmails_ReturnsOkResult_WhenServiceSucceds()
        {
            int pageSize = 100;
            int skip = 0;
            int total = 1000;
            int nextSkip = skip + pageSize >= total ? 0 : skip + pageSize;
            var expectedResult = GetEmailPaginator(pageSize, skip, total);
            _mockService.Setup(service => service.GetAllAsync(skip, pageSize)).ReturnsAsync(expectedResult);

            IActionResult result = await _emailControllerSut.GetAll(skip, pageSize);

            OkObjectResult okObjectResult = Assert.IsType<OkObjectResult>(result);
            var actualResult = okObjectResult.Value as EmailPaginator;
            Assert.Equal(200, okObjectResult.StatusCode);
            Assert.NotNull(okObjectResult);
            Assert.Equal(expectedResult, okObjectResult.Value);
            Assert.NotNull(actualResult);
            Assert.Equal(pageSize, actualResult.PageSize);
            Assert.Equal(nextSkip, actualResult.NextSkip);
            Assert.NotEqual(0, actualResult.Total);
            Assert.Equal(total, actualResult.Total);
            Assert.True(actualResult.Entities.Any());
        }

        [Fact(DisplayName = "Get All Emails with skip and pageSize null should return bad request")]
        public async Task GetAllEmails_ReturnsBadRequest_WhenServiceThrowsException()
        {
            string expectedResult = "Invalid parameters: skip is null, pageSize is null";
            _mockService.Setup(service => service.GetAllAsync(It.IsAny<int>(), It.IsAny<int>())).Throws(new Exception(expectedResult));

            IActionResult result = await _emailControllerSut.GetAll(It.IsAny<int>(), It.IsAny<int>());

            BadRequestObjectResult badRequestObjectResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.NotNull(badRequestObjectResult);
            Assert.Equal(expectedResult, badRequestObjectResult.Value);
        }

        [Fact(DisplayName = "Get By Id should return a valid email")]
        public async Task GetById_ReturnsOkResult_WhenServiceSucceds()
        {
            string id = Guid.NewGuid().ToString();
            var expectedResult = new EmailDto(id, "john@doe.com", "mary@anne.com", "Test", "<h1>Hello from XUnit</h1>", DateTime.Now);
            _mockService.Setup(service => service.GetByIdAsync(id)).ReturnsAsync(expectedResult);

            IActionResult result = await _emailControllerSut.GetById(id);

            OkObjectResult okObjectResult = Assert.IsType<OkObjectResult>(result);
            var actualResult = okObjectResult.Value as EmailDto;
            Assert.Equal(200, okObjectResult.StatusCode);
            Assert.NotNull(okObjectResult);
            Assert.Equal(expectedResult, okObjectResult.Value);
            Assert.IsType<EmailDto>(okObjectResult.Value);
            Assert.NotNull(actualResult);
            Assert.Equal(expectedResult.id, actualResult.id);
            Assert.Equal(expectedResult.from, actualResult.from);
            Assert.Equal(expectedResult.subject, actualResult.subject);
            Assert.Equal(expectedResult.body, actualResult.body);
            Assert.Equal(expectedResult.date, actualResult.date);
        }

        [Fact(DisplayName = "Get By Id if parameter is empty should return throws exception")]
        public async Task GetById_ReturnsBadRequest_WhenServiceThrowsException()
        {
            string id = string.Empty;
            string expectedResult = "The id must be informed";
            _mockService.Setup(service => service.GetByIdAsync(id)).Throws(new Exception(expectedResult));

            IActionResult result = await _emailControllerSut.GetById(id);

            BadRequestObjectResult badRequestObjectResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.NotNull(badRequestObjectResult);
            Assert.Equal(400, badRequestObjectResult.StatusCode);
            Assert.Equal(expectedResult, badRequestObjectResult.Value);
        }

        [Fact(DisplayName = "Delete By Id should return ok when passing a valid id and it is deleted")]
        public async Task DeleteById_ReturnsOkObjectResult_WhenServiceSucceds()
        {
            string id = Guid.NewGuid().ToString();
            string expectedResult = "Email successfully removed";
            _mockService.Setup(service => service.DeleteByIdAsync(id)).ReturnsAsync(expectedResult);

            IActionResult result = await _emailControllerSut.DeleteById(id);

            OkObjectResult okObjectResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, okObjectResult.StatusCode);
            Assert.NotNull(okObjectResult);
            Assert.Equal(expectedResult, okObjectResult.Value);

            string expectedResultgetById = "Email not found";
            _mockService.Setup(service => service.GetByIdAsync(id)).Throws(new NotFoundException(expectedResultgetById));

            IActionResult resultGetById = await _emailControllerSut.GetById(id);

            NotFoundObjectResult notFoundObjectResult = Assert.IsType<NotFoundObjectResult>(resultGetById);
            Assert.Equal(404, notFoundObjectResult.StatusCode);
            Assert.NotNull(notFoundObjectResult);
            Assert.Equal(expectedResultgetById, notFoundObjectResult.Value);
        }

        [Fact(DisplayName = "Delete All Emails should return all emails if request succeds")]
        public async Task DeleteAllEmails_ReturnsOkResult_WhenServiceSucceds()
        {
            string expectedResult = "All emails successfully removed.";
            _mockService.Setup(service => service.DeleteAllAsync()).ReturnsAsync(expectedResult);

            IActionResult result = await _emailControllerSut.DeleteAll();

            OkObjectResult okObjectResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, okObjectResult.StatusCode);
            Assert.NotNull(okObjectResult);
            Assert.Equal(expectedResult, okObjectResult.Value);
        }

        [Fact(DisplayName = "Delete all emails should return no emails found if all emails have already been removed")]
        public async Task DeleteAllEmails_ReturnsOkResult_WhenServiceNoFoundEmails()
        {
            string expectedResult = "No emails found";
            _mockService.Setup(service => service.DeleteAllAsync()).ReturnsAsync(expectedResult);

            IActionResult result = await _emailControllerSut.DeleteAll();

            OkObjectResult okObjectResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, okObjectResult.StatusCode);
            Assert.NotNull(okObjectResult);
            Assert.Equal(expectedResult, okObjectResult.Value);
        }

        [Fact(DisplayName = "Delete All Emails should throws exception if bad request")]
        public async Task DeleteAllEmails_ReturnsBadRequest_WhenServiceThrowsException()
        {
            string expectedResult = "No emails found";
            _mockService.Setup(service => service.DeleteAllAsync()).Throws(new Exception(expectedResult));

            IActionResult result = await _emailControllerSut.DeleteAll();

            BadRequestObjectResult badRequestObjectResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(400, badRequestObjectResult.StatusCode);
            Assert.NotNull(badRequestObjectResult);
            Assert.Equal(expectedResult, badRequestObjectResult.Value);
        }

        private EmailPaginator GetEmailPaginator(int pageSize, int skip, int total)
        {
            var emails = Enumerable.Range(0, pageSize).Select(i => new Email { Id = new MongoDB.Bson.ObjectId(), Inbox = $"to{i}@email.com", EmailFrom = $"from{i}@email.com", Subject = $"Subject - {i}", Body = $"Body{i}" });
            return new EmailPaginator(emails, pageSize, skip, total);
        }
    }
}
