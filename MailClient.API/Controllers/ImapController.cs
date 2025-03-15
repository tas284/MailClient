using Microsoft.AspNetCore.Mvc;
using MailClient.Application.Interfaces;
using MailClient.Application.InputModel;
using System;

namespace MailClient.Application.Controllers
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
    }
}
