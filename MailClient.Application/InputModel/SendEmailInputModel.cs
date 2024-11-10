using System.Net.Mail;
using System.Text.Json.Serialization;

namespace MailClient.Application.InputModel
{
    public class SendEmailInputModel
    {
        public string User { get; set; }
        public string Password { get; set; }
        public string FromEmail { get; set; }
        public string FromName { get; set; }
        public string ToEmail { get; set; }
        public string ToName { get; set; }
        public string SmtpAddress { get; set; }
        public int SmtpPort { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public string BodyHtml { get; set; }

        [JsonIgnore]
        public string Validations { get; set; }
        public bool IsValid()
        {
            if (string.IsNullOrEmpty(SmtpAddress) || SmtpPort <= 0)
                Validations = "Invalid SMTP:PORT Adrress";

            if (string.IsNullOrEmpty(User) || string.IsNullOrEmpty(Password))
                Validations = "Invalid user and password";

            if (string.IsNullOrEmpty(FromEmail) || !IsValidEmail(FromEmail))
                Validations = "Invalid sender email address";

            if (string.IsNullOrEmpty(ToEmail) || !IsValidEmail(ToEmail))
                Validations = "Invalid recipient email address";

            if (string.IsNullOrEmpty(Subject))
                Validations = "Subject cannot be empty";

            if (string.IsNullOrEmpty(Body) && string.IsNullOrEmpty(BodyHtml))
                Validations = "At least one body (text or HTML) must be provided";

            return string.IsNullOrEmpty(Validations);
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
