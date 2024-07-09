using MailClient.InputModel;

namespace MailClient.Interfaces
{
    public interface IEmailImapService
    {
        Task<string> SyncMessages(SyncEmailInputModel input);
    }
}
