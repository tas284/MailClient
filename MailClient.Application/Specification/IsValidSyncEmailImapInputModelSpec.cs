using MailClient.Application.InputModel;
using MailClient.Application.Interfaces;
using System.Net.Mail;

namespace MailClient.Application.Specification
{
    public class IsValidSyncEmailImapInputModelSpec : ISpecification<SyncEmailImapInputModel>
    {
        public bool IsSatifiedBy(SyncEmailImapInputModel inputModel)
        {
            if (string.IsNullOrEmpty(inputModel.ImapAddress) || inputModel.ImapPort <= 0)
                inputModel.Validations = "Invalid IMAP:PORT Adrress";

            if (string.IsNullOrEmpty(inputModel.User) || string.IsNullOrEmpty(inputModel.Password) || !IsValidEmail(inputModel.User))
                inputModel.Validations = "Invalid user and password";

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
