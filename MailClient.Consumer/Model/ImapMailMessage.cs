namespace MailClient.Consumer.Model;
public record ImapMailMessage(string EmailTo, string EmailFrom, string Subject, string Body, DateTime Date);
