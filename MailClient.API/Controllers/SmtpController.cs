using MailClient.Application.InputModel;
using MailClient.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace MailClient.Application.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SmtpController : ControllerBase
    {
        private readonly IEmailSmtpService _service;

        public SmtpController(IEmailSmtpService service)
        {
            _service = service;
        }

        [HttpPost]
        public IActionResult SendEmail([FromBody] SendEmailInputModel input)
        {
            var result = _service.Send(input);
            return Ok(result);
        }
    }
}
