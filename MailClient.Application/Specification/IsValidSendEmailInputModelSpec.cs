using MailClient.Application.InputModel;
using MailClient.Application.Interfaces;
using System.Net.Mail;

namespace MailClient.Application.Specification
{
    public class IsValidSendEmailInputModelSpec : ISpecification<SendEmailInputModel>
    {
        public bool IsSatisfiedBy(SendEmailInputModel inputModel)
        {
            if (string.IsNullOrEmpty(inputModel.SmtpAddress) || inputModel.SmtpPort <= 0)
                inputModel.Validations = "Invalid SMTP:PORT Adrress";

            if (string.IsNullOrEmpty(inputModel.User) || string.IsNullOrEmpty(inputModel.Password))
                inputModel.Validations = "Invalid user and password";

            if (string.IsNullOrEmpty(inputModel.FromEmail) || !IsValidEmail(inputModel.FromEmail))
                inputModel.Validations = "Invalid sender email address";

            if (string.IsNullOrEmpty(inputModel.ToEmail) || !IsValidEmail(inputModel.ToEmail))
                inputModel.Validations = "Invalid recipient email address";

            if (string.IsNullOrEmpty(inputModel.Subject))
                inputModel.Validations = "Subject cannot be empty";

            if (string.IsNullOrEmpty(inputModel.Body) && string.IsNullOrEmpty(inputModel.BodyHtml))
                inputModel.Validations = "At least one body (text or HTML) must be provided";

            return string.IsNullOrEmpty(inputModel.Validations);
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var mailAddress = new MailAddress(email);
                return mailAddress.Address == email;
            }
            catch
            {
                return false;
            }
        }
    }
}
