using MailClient.API.InputModel;
using MailClient.API.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace MailClient.API.Controllers
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
