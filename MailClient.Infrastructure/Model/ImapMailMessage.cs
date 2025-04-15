using MimeKit;

namespace MailClient.Infrastructure.Model
{
    public record ImapMailMessage(string EmailTo, string EmailFrom, string Subject, string Body, string MessageId, DateTime Date)
    {
        public static ImapMailMessage Create(MimeMessage message)
        {
            var to = message.To.Mailboxes?.FirstOrDefault()?.Address ?? string.Empty;
            var from = message.From.Mailboxes.FirstOrDefault()?.Address ?? string.Empty;
            return new ImapMailMessage(to, from, message.Subject, message.HtmlBody, message.MessageId, message.Date.LocalDateTime);
        }
    }
}