using MailClient.InputModel;

namespace MailClient.Interfaces
{
    public interface IMailService
    {
        Task<bool> SyncEmail(SyncEmailInputModel input);
    }
}
