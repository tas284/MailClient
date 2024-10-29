namespace MailClient.Application.Model
{
    public record InputImapMail(string Inbox, string EmailFrom, string Subject, string Body, DateTime Date);
}
