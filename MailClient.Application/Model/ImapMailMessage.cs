using MimeKit;

namespace MailClient.Application.Model
{
    public record ImapMailMessage(string EmailTo, string EmailFrom, string Subject, string Body, DateTime Date)
    {
        public static ImapMailMessage Create(MimeMessage message)
        {
            return new ImapMailMessage(message.To.Mailboxes?.FirstOrDefault()?.Address, message.From.Mailboxes.FirstOrDefault()!.Address, message.Subject, message.HtmlBody, message.Date.LocalDateTime);
        }
    }
}
