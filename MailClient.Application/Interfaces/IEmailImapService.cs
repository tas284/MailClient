using MailClient.Application.InputModel;

namespace MailClient.Application.Interfaces
{
    public interface IEmailImapService
    {
        string SyncMessages(ImapInputModel input);
        Task<string> SyncMessagesBatch(ImapInputModel input);
    }
}
