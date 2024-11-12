using MailClient.Application.Controllers;
using MailClient.Application.DTO;
using MailClient.Application.Exceptions;
using MailClient.Application.Interfaces;
using MailClient.Application.Paginator;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace MailClient.API.Test.Controllers
{
    public class EmailControllerTest
    {
        private readonly Mock<IEmailService> _mockService;
        private readonly EmailController _emailController;

        public EmailControllerTest()
        {
            _mockService = new Mock<IEmailService>();
            _emailController = new EmailController(_mockService.Object);
        }

        [Fact(DisplayName = "Get All Emails should return all emails according to the given skip and pageSize")]
        public async Task GetAllEmails_ReturnsOkResult_WhenServiceSucceds()
        {
            int pageSize = 1000;
            int skip = 0;
            var emails = Enumerable.Range(0, pageSize).Select(i => new EmailDto($"email - {i}", $"from{i}@email.com", $"to{i}@email.com", $"Subject - {i}", $"Body{i}", DateTime.Now.AddMinutes(-i)));
            EmailPaginator expectedResult = new()
            {
                PageSize = pageSize,
                NextSkip = skip + pageSize,
                Total = emails.Count(),
                Emails = emails
            };
            _mockService.Setup(service => service.GetAllAsync(skip, pageSize)).ReturnsAsync(expectedResult);

            IActionResult result = await _emailController.GetAll(skip, pageSize);

            OkObjectResult okObjectResult = Assert.IsType<OkObjectResult>(result);
            var actualResult = okObjectResult.Value as EmailPaginator;
            Assert.Equal(200, okObjectResult.StatusCode);
            Assert.NotNull(okObjectResult);
            Assert.Equal(expectedResult, okObjectResult.Value);
            Assert.NotNull(actualResult);
            Assert.Equal(pageSize, actualResult.PageSize);
            Assert.Equal(skip + pageSize, actualResult.NextSkip);
            Assert.NotEqual(0, actualResult.Total);
            Assert.Equal(emails.Count(), actualResult.Total);
            Assert.True(actualResult.Emails.Any());
        }

<<<<<<< Updated upstream
        [Fact(DisplayName = "Get All Emails with skip and pageSize null should return badRequest")]
=======
        [Fact(DisplayName = "Get All Emails with skip and pageSize null should return bad request")]
>>>>>>> Stashed changes
        public async Task GetAllEmails_ReturnsBadRequest_WhenServiceThrowsException()
        {
            string expectedResult = "Invalid parameters: skip is null, pageSize is null";
            _mockService.Setup(service => service.GetAllAsync(It.IsAny<int>(), It.IsAny<int>())).Throws(new Exception(expectedResult));

            IActionResult result = await _emailController.GetAll(It.IsAny<int>(), It.IsAny<int>());

            BadRequestObjectResult badRequestObjectResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.NotNull(badRequestObjectResult);
            Assert.Equal(expectedResult, badRequestObjectResult.Value);
        }

<<<<<<< Updated upstream
        [Fact(DisplayName = "Get By Id must return a valid email")]
=======
        [Fact(DisplayName = "Get By Id should return a valid email")]
>>>>>>> Stashed changes
        public async Task GetById_ReturnsOkResult_WhenServiceSucceds()
        {
            string id = Guid.NewGuid().ToString();
            var expectedResult = new EmailDto(id, "john@doe.com", "mary@anne.com", "Test", "<h1>Hello from XUnit</h1>", DateTime.Now);
            _mockService.Setup(service => service.GetByIdAsync(id)).ReturnsAsync(expectedResult);

            IActionResult result = await _emailController.GetById(id);

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

<<<<<<< Updated upstream
        [Fact(DisplayName = "Get By Empty Id throws Exception")]
=======
        [Fact(DisplayName = "Get By Id if parameter is empty should return throws exception")]
>>>>>>> Stashed changes
        public async Task GetById_ReturnsBadRequest_WhenServiceThrowsException()
        {
            string id = string.Empty;
            string expectedResult = "The id must be informed";
            _mockService.Setup(service => service.GetByIdAsync(id)).Throws(new Exception(expectedResult));

            IActionResult result = await _emailController.GetById(id);

            BadRequestObjectResult badRequestObjectResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.NotNull(badRequestObjectResult);
            Assert.Equal(400, badRequestObjectResult.StatusCode);
            Assert.Equal(expectedResult, badRequestObjectResult.Value);
        }

<<<<<<< Updated upstream
        [Fact(DisplayName = "Delete By Id must return Ok when passing a valid id and it is deleted")]
=======
        [Fact(DisplayName = "Delete By Id should return ok when passing a valid id and it is deleted")]
>>>>>>> Stashed changes
        public async Task DeleteById_ReturnsOkObjectResult_WhenServiceSucceds()
        {
            string id = Guid.NewGuid().ToString();
            string expectedResult = "Email successfully removed";
            _mockService.Setup(service => service.DeleteByIdAsync(id)).ReturnsAsync(expectedResult);

            IActionResult result = await _emailController.DeleteById(id);

            OkObjectResult okObjectResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, okObjectResult.StatusCode);
            Assert.NotNull(okObjectResult);
            Assert.Equal(expectedResult, okObjectResult.Value);

            string expectedResultgetById = "Email not found";
            _mockService.Setup(service => service.GetByIdAsync(id)).Throws(new NotFoundException(expectedResultgetById));

            IActionResult resultGetById = await _emailController.GetById(id);

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

            IActionResult result = await _emailController.DeleteAll();

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

            IActionResult result = await _emailController.DeleteAll();

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

            IActionResult result = await _emailController.DeleteAll();

            BadRequestObjectResult badRequestObjectResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(400, badRequestObjectResult.StatusCode);
            Assert.NotNull(badRequestObjectResult);
            Assert.Equal(expectedResult, badRequestObjectResult.Value);
        }
    }
}
