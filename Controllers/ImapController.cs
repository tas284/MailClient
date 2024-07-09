using MailClient.InputModel;
using MailClient.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace MailClient.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImapController : ControllerBase
    {
        private readonly IMailService _emailService;
        public ImapController(IMailService emailService)
        {
            _emailService = emailService;
        }

        [HttpPost]
        public async Task<IActionResult> Sync([FromBody] SyncEmailInputModel inputModel)
        {
            var result = await _emailService.SyncEmail(inputModel).ConfigureAwait(false);
            return Ok(result);
        }
    }
}
