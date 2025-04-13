using Microsoft.AspNetCore.Mvc;
using MailClient.Application.Interfaces;
using MailClient.Application.InputModel;
using System;
using System.Threading.Tasks;

namespace MailClient.Application.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ImapController : ControllerBase
    {
        private readonly IEmailImapService _service;

        public ImapController(IEmailImapService service)
        {
            _service = service;
        }

        [HttpPost]
        public IActionResult SyncMessages([FromBody] SyncEmailImapInputModel inputModel)
        {
            try
            {
                var result = _service.SyncMessages(inputModel);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> SyncMessagesBatch([FromBody] SyncEmailImapInputModel inputModel)
        {
            try
            {
                var result = await _service.SyncMessagesBatch(inputModel);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
