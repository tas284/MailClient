using MailClient.Application.InputModel;

namespace MailClient.Application.Interfaces
{
    public interface IEmailImapService
    {
        Task<string> SyncMessages(SyncEmailImapInputModel input);
    }
}
