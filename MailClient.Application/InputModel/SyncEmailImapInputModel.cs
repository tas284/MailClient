using System.Net.Mail;
using System.Text.Json.Serialization;

namespace MailClient.Application.InputModel
{
    public class SyncEmailImapInputModel
    {
        public string User { get; set; }
        public string Password { get; set; }
        public string ImapAddress { get; set; }
        public int ImapPort { get; set; }
        public DateTime StartDate { get; set; }

        [JsonIgnore]
        public DateTime EndDate { get; set; }
        [JsonIgnore]
        public string Validations { get; set; }

        public bool IsValid()
        {
            if (string.IsNullOrEmpty(ImapAddress) || ImapPort <= 0)
                Validations = "Invalid IMAP:PORT Adrress";

            if (string.IsNullOrEmpty(User) || string.IsNullOrEmpty(Password) || !IsValidEmail(User))
                Validations = "Invalid user and password";

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
