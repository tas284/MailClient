using MailClient.InputModel;
using MailClient.Interfaces;
using MailClient.Services;
using Microsoft.AspNetCore.Mvc;

namespace MailClient.Controllers
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
        public async Task<IActionResult> SyncMessages([FromBody] SyncEmailInputModel inputModel)
        {
            var result = await _emailService.SyncMessages(inputModel).ConfigureAwait(false);
            return Ok(result);
        }
    }
}
