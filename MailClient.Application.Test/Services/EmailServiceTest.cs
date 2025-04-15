using MailClient.Application.DTO;
using MailClient.Application.Exceptions;
using MailClient.Application.Interfaces;
using MailClient.Application.Paginator;
using MailClient.Application.Services;
using MailClient.Domain.Entities;
using MailClient.Domain.Repositories;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using Moq;

namespace MailClient.Application.Test.Services
{
    public class EmailServiceTest
    {
        private readonly Mock<ILogger<EmailService>> _mockLogger;
        private readonly Mock<IRepositoryEmail> _mockRepositoryEmail;
        private readonly EmailService _emailServiceSut;

        public EmailServiceTest()
        {
            _mockLogger = new Mock<ILogger<EmailService>>();
            _mockRepositoryEmail = new Mock<IRepositoryEmail>();
            _emailServiceSut = new EmailService(_mockRepositoryEmail.Object, _mockLogger.Object);
        }

        [Fact(DisplayName = "Get By Id Async should return a EmailDto when successfully")]
        public async Task GetByIdAsync_ShouldReturnEmailDto_WhenSuccessfully()
        {
            EmailDto expectedResult = GetEmailDto();
            string id = expectedResult.id;
            _mockRepositoryEmail.Setup(repository => repository.GetByIdAsync(id)).ReturnsAsync(Email);

            EmailDto actualResult = await _emailServiceSut.GetByIdAsync(id);

            Assert.NotNull(actualResult);
            Assert.Equal(expectedResult, actualResult);
            Assert.Equal(expectedResult.id, actualResult.id);
            Assert.Equal(expectedResult.to, actualResult.to);
            Assert.Equal(expectedResult.from, actualResult.from);
            Assert.Equal(expectedResult.subject, actualResult.subject);
            Assert.Equal(expectedResult.body, actualResult.body);
            _mockRepositoryEmail.Verify(repository => repository.GetByIdAsync(id), Times.Once);
        }

        [Fact(DisplayName = "Get By Id Async should trows argument exception if id is empty")]
        public async Task GetByIdAsync_ShouldThrowsArgumentException_IfIdIsEmpty()
        {
            string expectedResult = "The id must be informed";
            string id = string.Empty;

            var actualResult = await Assert.ThrowsAsync<ArgumentException>(() => _emailServiceSut.GetByIdAsync(id));

            Assert.NotNull(actualResult);
            Assert.Equal(expectedResult, actualResult.Message);
            _mockRepositoryEmail.Verify(repository => repository.GetByIdAsync(id), Times.Never);
        }

        [Fact(DisplayName = "Get By Id Async should trows not found exception if id not found into database")]
        public async Task GetByIdAsync_ShouldThrowsNotFoundException_IfIdNotFoundIntoDatabase()
        {
            string id = Guid.NewGuid().ToString();
            string exptectedResult = $"Email not found in the database: {id}";

            var actualResult = await Assert.ThrowsAsync<NotFoundException>(() => _emailServiceSut.GetByIdAsync(id));

            Assert.NotNull(actualResult);
            Assert.Equal(exptectedResult, actualResult.Message);
        }

        [Fact(DisplayName = "Get All Async should return a EmailDto when successfully")]
        public async Task GetAllAsync_ShouldReturnEmailPaginator_WhenSuccessfully()
        {
            int skip = 100;
            int pageSize = 100;
            int total = 1000;
            int nextSkip = skip + pageSize >= total ? 0 : skip + pageSize;
            IEnumerable<Email> emails = GetEmails(pageSize);
            _mockRepositoryEmail.Setup(repository => repository.GetAllAsync(skip, pageSize)).ReturnsAsync(emails);
            _mockRepositoryEmail.Setup(repository => repository.CountAsync(_ => true)).ReturnsAsync(total);

            EmailPaginator actualResult = await _emailServiceSut.GetAllAsync(skip, pageSize);

            Assert.NotNull(actualResult);
            Assert.Equal(pageSize, actualResult.PageSize);
            Assert.Equal(nextSkip, actualResult.NextSkip);
            Assert.Equal(total, actualResult.Total);
            _mockRepositoryEmail.Verify(repository => repository.GetAllAsync(skip, pageSize), Times.Once);
            _mockRepositoryEmail.Verify(repository => repository.CountAsync(_ => true), Times.Once);
        }

        [Fact(DisplayName = "Get All Async should trows argument exception if pageSize is zero")]
        public async Task GetAllAsync_ShouldThrowsArgumentException_IfPageSizeIsEmpty()
        {
            string expectedResult = "The pageSize must be grather than zero";
            int skip = 0;
            int pageSize = 0;

            var actualResult = await Assert.ThrowsAsync<ArgumentException>(() => _emailServiceSut.GetAllAsync(skip, pageSize));

            Assert.NotNull(actualResult);
            Assert.Equal(expectedResult, actualResult.Message);
            _mockRepositoryEmail.Verify(repository => repository.GetAllAsync(skip, pageSize), Times.Never);
        }

        [Fact(DisplayName = "Get All Async should trows not found exception if no emails are found in the database")]
        public async Task GetAllAsync_ShouldThrowsNotFoundException_IfNoEmailsAreFoundInTheDatabase()
        {
            string expectedResult = "No emails found in the database";
            int skip = 0;
            int pageSize = 10;

            var actualResult = await Assert.ThrowsAsync<NotFoundException>(() => _emailServiceSut.GetAllAsync(skip, pageSize));

            Assert.NotNull(actualResult);
            Assert.Equal(expectedResult, actualResult.Message);
            _mockRepositoryEmail.Verify(repository => repository.GetAllAsync(skip, pageSize), Times.Once);
            _mockRepositoryEmail.Verify(repository => repository.CountAsync(_ => true), Times.Never);
        }

        [Fact(DisplayName = "Delete By Id Async should return success when email is removed from the database")]
        public async Task DeleteByIdAsync_ShouldReturnSuccess_WhenEmailIsRemovedFromDatabase()
        {
            string expectedResult = "Email successfully removed";
            string id = Guid.NewGuid().ToString();
            _mockRepositoryEmail.Setup(repository => repository.DeleteByIdAsync(id)).ReturnsAsync(true);

            var actualResult = await _emailServiceSut.DeleteByIdAsync(id);

            Assert.NotNull(actualResult);
            Assert.Equal(expectedResult, actualResult);
            _mockRepositoryEmail.Verify(repository => repository.DeleteByIdAsync(id), Times.Once);
        }

        [Fact(DisplayName = "Delete By Id Async should throws not found exception when email not exists in the database")]
        public async Task DeleteByIdAsync_ShouldThrowsNotFoundExcepetion_WhenEmailNotExistsInTheDatabase()
        {
            string id = Guid.NewGuid().ToString();
            string expectedResult = $"No email found with this Id {id}";
            _mockRepositoryEmail.Setup(repository => repository.DeleteByIdAsync(id)).ReturnsAsync(false);

            var actualResult = await Assert.ThrowsAsync<NotFoundException>(() => _emailServiceSut.DeleteByIdAsync(id));

            Assert.NotNull(actualResult);
            Assert.Equal(expectedResult, actualResult.Message);
            _mockRepositoryEmail.Verify(repository => repository.DeleteByIdAsync(id), Times.Once);
        }

        [Fact(DisplayName = "Delete By Id Async should throws argument exception when id is empty")]
        public async Task DeleteByIdAsync_ShouldThrowsArgumentException_IfIdIsEmpty()
        {
            string expectedResult = "The id is required to remove from database";
            string id = string.Empty;

            var actualResult = await Assert.ThrowsAsync<ArgumentException>(() => _emailServiceSut.DeleteByIdAsync(id));

            Assert.NotNull(actualResult);
            Assert.Equal(expectedResult, actualResult.Message);
            _mockRepositoryEmail.Verify(repository => repository.DeleteByIdAsync(id), Times.Never);
        }

        [Fact(DisplayName = "Delete All Async should return success when all emails are removed from database")]
        public async Task DelteAllAsync_ShouldReturnSuccess_WhenAllEmailsAreRemovedFromDatabase()
        {
            int count = 100;
            string expectedResult = $"{count} emails successfully removed";

            _mockRepositoryEmail.Setup(repository => repository.DeleteAllAsync()).ReturnsAsync(count);

            var actualResult = await _emailServiceSut.DeleteAllAsync();

            Assert.NotNull(actualResult);
            Assert.Equal(expectedResult, actualResult);
            _mockRepositoryEmail.Verify(repository => repository.DeleteAllAsync(), Times.Once);
        }

        [Fact(DisplayName = "Delete All Async should throws not found exception when no emails are found in the database")]
        public async Task DelteAllAsync_ShouldThrowsNotFoundException_WhenNoEmailsAreFoundInTheDatabase()
        {
            int count = 0;
            string expectedResult = "No emails were found to delete";

            _mockRepositoryEmail.Setup(repository => repository.DeleteAllAsync()).ReturnsAsync(count);

            var actualResult = await Assert.ThrowsAsync<NotFoundException>(() => _emailServiceSut.DeleteAllAsync());

            Assert.NotNull(actualResult);
            Assert.Equal(expectedResult, actualResult.Message);
            _mockRepositoryEmail.Verify(repository => repository.DeleteAllAsync(), Times.Once);
        }

        private EmailDto GetEmailDto() => EmailDto.Create(Email());

        private Email Email() => new Email
        {
            Id = new MongoDB.Bson.ObjectId("6732a8af452b16c289d293b4"),
            Inbox = "john@doe.com",
            EmailFrom = "mary@doe.com",
            Subject = "Test with xUnit",
            Body = "Hello from test!"
        };

        private IEnumerable<Email> GetEmails(int pageSize)
        {
            IEnumerable<Email> emails = Enumerable.Range(0, pageSize).Select(i => new Email {
                Id = new ObjectId(),
                Inbox = "john@doe.com",
                EmailFrom = "mary@doe.com",
                Subject = "Test with xUnit",
                Body = "Hello from test!"
            });

            return emails;
        }
    }
}
