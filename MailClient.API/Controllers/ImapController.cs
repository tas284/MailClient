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
        private readonly IEmailImapService _service;

        public ImapController(IEmailImapService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<IActionResult> SyncMessages([FromBody] SyncEmailImapInputModel inputModel)
        {
            var result = await _service.SyncMessages(inputModel);
            return Ok(result);
        }
    }
}
