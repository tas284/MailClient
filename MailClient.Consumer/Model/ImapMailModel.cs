namespace MailClient.Consumer.Model
{
    public record ImapMailModel(string Inbox, string EmailFrom, string Subject, string Body, DateTime Date);
}
