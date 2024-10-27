using System;

namespace MailClient.API.Model
{
    public record InputImapMail(string Inbox, string EmailFrom, string Subject, string Body, DateTime Date);
}
