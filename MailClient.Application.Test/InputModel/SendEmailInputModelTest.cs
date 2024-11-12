using MailClient.Application.InputModel;
using MailClient.Domain.Entities;
using Moq;

namespace MailClient.Application.Test.InputModel
{
    public class SendEmailInputModelTest
    {
        public SendEmailInputModelTest() { }

<<<<<<< Updated upstream
        [Fact(DisplayName = "Send Should Return Success When SendEmailInputModel is Valid")]
=======
        [Fact(DisplayName = "Send should return success when SendEmailInputModel is Valid")]
>>>>>>> Stashed changes
        public void IsValid_ShouldReturnTrue_WhenSendEmailInputModelIsValid()
        {
            SendEmailInputModel sendEmailInputModel = GetSendEmailInputModel();

            bool isValid = sendEmailInputModel.IsValid();

            Assert.Null(sendEmailInputModel.Validations);
            Assert.True(isValid);
        }

<<<<<<< Updated upstream
        [Fact(DisplayName = "Send Should Return False When Smpt Address Or Smtp Port is Invalid")]
=======
        [Fact(DisplayName = "Send should return false when Smpt Address Or Smtp Port is Invalid")]
>>>>>>> Stashed changes
        public void IsValid_ShouldReturnFalse_WhenSmptAddressOrSmtpPortIsInvalid()
        {
            SendEmailInputModel sendEmailInputModel = GetSendEmailInputModel();
            sendEmailInputModel.SmtpPort = 0;
            sendEmailInputModel.SmtpAddress = null;
            string expectedResult = "Invalid SMTP:PORT Adrress";

            bool isValid = sendEmailInputModel.IsValid();

            Assert.Equal(expectedResult, sendEmailInputModel.Validations);
            Assert.False(isValid);
        }

<<<<<<< Updated upstream
        [Fact(DisplayName = "Send Should Return False When User or Password is Invalid")]
=======
        [Fact(DisplayName = "Send should return false when User or Password is Invalid")]
>>>>>>> Stashed changes
        public void IsValid_ShouldReturnFalse_WhenUserOrPasswordIsInvalid()
        {
            SendEmailInputModel sendEmailInputModel = GetSendEmailInputModel();
            sendEmailInputModel.User = sendEmailInputModel.Password = null;
            string expectedResult = "Invalid user and password";

            bool isValid = sendEmailInputModel.IsValid();

            Assert.Equal(expectedResult, sendEmailInputModel.Validations);
            Assert.False(isValid);
        }

<<<<<<< Updated upstream
        [Fact(DisplayName = "Send Should Return False When To Email Or To Name is Invalid")]
=======
        [Fact(DisplayName = "Send should return false when To Email or To Name is Invalid")]
>>>>>>> Stashed changes
        public void IsValid_ShouldReturnFalse_WhenToEmailOrToNameIsInvalid()
        {
            SendEmailInputModel sendEmailInputModel = GetSendEmailInputModel();
            sendEmailInputModel.ToEmail = sendEmailInputModel.ToName = null;
            string expectedResult = "Invalid recipient email address";

            bool isValid = sendEmailInputModel.IsValid();

            Assert.Equal(expectedResult, sendEmailInputModel.Validations);
            Assert.False(isValid);
        }

<<<<<<< Updated upstream
        [Fact(DisplayName = "Send Should Return False When From Email Or From Name is Invalid")]
=======
        [Fact(DisplayName = "Send should return false when From Email or From Name is Invalid")]
>>>>>>> Stashed changes
        public void IsValid_ShouldReturnFalse_WhenFromEmailOrFromIsInvalid()
        {
            SendEmailInputModel sendEmailInputModel = GetSendEmailInputModel();
            sendEmailInputModel.FromEmail = sendEmailInputModel.FromName = null;
            string expectedResult = "Invalid sender email address";

            bool isValid = sendEmailInputModel.IsValid();

            Assert.Equal(expectedResult, sendEmailInputModel.Validations);
            Assert.False(isValid);
        }

<<<<<<< Updated upstream
        [Fact(DisplayName = "Send Should Return False When Subject is Invalid")]
=======
        [Fact(DisplayName = "Send should return false when Subject is Invalid")]
>>>>>>> Stashed changes
        public void IsValid_ShouldReturnFalse_WhenSubjectIsInvalid()
        {
            SendEmailInputModel sendEmailInputModel = GetSendEmailInputModel();
            sendEmailInputModel.Subject = null;
            string expectedResult = "Subject cannot be empty";

            bool isValid = sendEmailInputModel.IsValid();

            Assert.Equal(expectedResult, sendEmailInputModel.Validations);
            Assert.False(isValid);
        }

<<<<<<< Updated upstream
        [Fact(DisplayName = "Send Should Return False When Body is Invalid")]
=======
        [Fact(DisplayName = "Send should return false when Body is Invalid")]
>>>>>>> Stashed changes
        public void IsValid_ShouldReturnFalse_WhenBodyIsInvalid()
        {
            SendEmailInputModel sendEmailInputModel = GetSendEmailInputModel();
            sendEmailInputModel.Body = sendEmailInputModel.BodyHtml = null;
            string expectedResult = "At least one body (text or HTML) must be provided";

            bool isValid = sendEmailInputModel.IsValid();

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
