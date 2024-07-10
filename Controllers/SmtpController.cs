using MailClient.InputModel;
using MailClient.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace MailClient.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SmtpController : ControllerBase
    {
        private readonly IEmailSmtpService _emailSmtpService;

        public SmtpController(IEmailSmtpService emailSmtpService)
        {
            _emailSmtpService = emailSmtpService;
        }

        [HttpPost]
        public async Task<IActionResult> SendEmail([FromBody] SendEmailInputModel input)
        {
            var result = await _emailSmtpService.Send(input);
            return Ok(result);
        }
    }
}
