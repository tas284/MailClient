using MimeKit;

namespace MailClient.Application.Model
{
    public record InputImapMail(string EmailTo, string EmailFrom, string Subject, string Body, DateTime Date)
    {
        public static InputImapMail Create(MimeMessage message)
        {
            return new InputImapMail(message.To.Mailboxes?.FirstOrDefault()?.Address, message.From.Mailboxes.FirstOrDefault()!.Address, message.Subject, message.HtmlBody, message.Date.LocalDateTime);
        }
    }
}
