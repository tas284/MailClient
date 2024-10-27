using MailClient.API.InputModel;
using System.Threading.Tasks;

namespace MailClient.API.Interfaces
{
    public interface IEmailImapService
    {
        Task<string> SyncMessages(SyncEmailImapInputModel input);
    }
}
