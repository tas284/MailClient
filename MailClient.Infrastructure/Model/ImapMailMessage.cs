using MimeKit;

namespace MailClient.Infrastructure.Model
{
    public record ImapMailMessage(string EmailTo, string EmailFrom, string Subject, string Body, DateTime Date)
    {
        public static ImapMailMessage Create(MimeMessage message)
        {
            var to = message.To.Mailboxes?.FirstOrDefault()!.Address;
            var from = message.From.Mailboxes.FirstOrDefault()!.Address;
            return new ImapMailMessage(to, from, message.Subject, message.HtmlBody, message.Date.LocalDateTime);
        }
    }
}