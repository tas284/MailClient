using MailClient.Application.InputModel;
using MailClient.Application.Interfaces;
using MailClient.Application.Specification;
using MailClient.Domain.Entities;
using Moq;

namespace MailClient.Application.Test.InputModel
{
    public class SendEmailInputModelTest
    {
        private readonly ISpecification<SendEmailInputModel> _spec;
        public SendEmailInputModelTest() 
        {
            _spec = new IsValidSendEmailInputModelSpec();
        }

        [Fact(DisplayName = "Send should return success when SendEmailInputModel is Valid")]
        public void IsValid_ShouldReturnTrue_WhenSendEmailInputModelIsValid()
        {
            SendEmailInputModel sendEmailInputModel = GetSendEmailInputModel();

            bool isValid = _spec.IsSatifiedBy(sendEmailInputModel);

            Assert.Null(sendEmailInputModel.Validations);
            Assert.True(isValid);
        }

        [Fact(DisplayName = "Send should return false when Smpt Address Or Smtp Port is Invalid")]
        public void IsValid_ShouldReturnFalse_WhenSmptAddressOrSmtpPortIsInvalid()
        {
            SendEmailInputModel sendEmailInputModel = GetSendEmailInputModel();
            sendEmailInputModel.SmtpPort = 0;
            sendEmailInputModel.SmtpAddress = null;
            string expectedResult = "Invalid SMTP:PORT Adrress";

            bool isValid = _spec.IsSatifiedBy(sendEmailInputModel);

            Assert.Equal(expectedResult, sendEmailInputModel.Validations);
            Assert.False(isValid);
        }

        [Fact(DisplayName = "Send should return false when User or Password is Invalid")]
        public void IsValid_ShouldReturnFalse_WhenUserOrPasswordIsInvalid()
        {
            SendEmailInputModel sendEmailInputModel = GetSendEmailInputModel();
            sendEmailInputModel.User = sendEmailInputModel.Password = null;
            string expectedResult = "Invalid user and password";

            bool isValid = _spec.IsSatifiedBy(sendEmailInputModel);

            Assert.Equal(expectedResult, sendEmailInputModel.Validations);
            Assert.False(isValid);
        }

        [Fact(DisplayName = "Send should return false when To Email or To Name is Invalid")]
        public void IsValid_ShouldReturnFalse_WhenToEmailOrToNameIsInvalid()
        {
            SendEmailInputModel sendEmailInputModel = GetSendEmailInputModel();
            sendEmailInputModel.ToEmail = sendEmailInputModel.ToName = null;
            string expectedResult = "Invalid recipient email address";

            bool isValid = _spec.IsSatifiedBy(sendEmailInputModel);

            Assert.Equal(expectedResult, sendEmailInputModel.Validations);
            Assert.False(isValid);
        }

        [Fact(DisplayName = "Send should return false when From Email or From Name is Invalid")]
        public void IsValid_ShouldReturnFalse_WhenFromEmailOrFromIsInvalid()
        {
            SendEmailInputModel sendEmailInputModel = GetSendEmailInputModel();
            sendEmailInputModel.FromEmail = sendEmailInputModel.FromName = null;
            string expectedResult = "Invalid sender email address";

            bool isValid = _spec.IsSatifiedBy(sendEmailInputModel);

            Assert.Equal(expectedResult, sendEmailInputModel.Validations);
            Assert.False(isValid);
        }

        [Fact(DisplayName = "Send should return false when Subject is Invalid")]
        public void IsValid_ShouldReturnFalse_WhenSubjectIsInvalid()
        {
            SendEmailInputModel sendEmailInputModel = GetSendEmailInputModel();
            sendEmailInputModel.Subject = null;
            string expectedResult = "Subject cannot be empty";

            bool isValid = _spec.IsSatifiedBy(sendEmailInputModel);

            Assert.Equal(expectedResult, sendEmailInputModel.Validations);
            Assert.False(isValid);
        }

        [Fact(DisplayName = "Send should return false when Body is Invalid")]
        public void IsValid_ShouldReturnFalse_WhenBodyIsInvalid()
        {
            SendEmailInputModel sendEmailInputModel = GetSendEmailInputModel();
            sendEmailInputModel.Body = sendEmailInputModel.BodyHtml = null;
            string expectedResult = "At least one body (text or HTML) must be provided";

            bool isValid = _spec.IsSatifiedBy(sendEmailInputModel);

            Assert.Equal(expectedResult, sendEmailInputModel.Validations);
            Assert.False(isValid);
        }

        #region Helpers
        public static SendEmailInputModel GetSendEmailInputModel()
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
