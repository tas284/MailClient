using MailClient.API.InputModel;
using MailClient.API.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace MailClient.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImapController : ControllerBase
    {
        private readonly IEmailImapService _emailService;
        public ImapController(IEmailImapService emailService)
        {
            _emailService = emailService;
        }

        [HttpPost]
        public async Task<IActionResult> SyncMessages([FromBody] SyncEmailImapInputModel inputModel)
        {
            var result = await _emailService.SyncMessages(inputModel);
            return Ok(result);
        }
    }
}
