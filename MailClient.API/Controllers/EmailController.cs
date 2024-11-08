using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MailClient.Application.Interfaces;
using MailClient.Application.DTO;
using MailClient.Application.Paginator;
using MailClient.Application.Exceptions;

namespace MailClient.Application.Controllers
{
    [Route("api/[controller]/")]
    [ApiController]
    public class EmailController : ControllerBase
    {
        private readonly IEmailService _service;

        public EmailController(IEmailService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(int skip, int pageSize = 20)
        {
            try
            {
                EmailPaginator result = await _service.GetAllAsync(skip, pageSize);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            try
            {
                EmailDto result = await _service.GetByIdAsync(id);
                return Ok(result);
            }
            catch (NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteById(string id)
        {
            try
            {
                string result = await _service.DeleteByIdAsync(id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteAll()
        {
            try
            {
                string result = await _service.DeleteAllAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
