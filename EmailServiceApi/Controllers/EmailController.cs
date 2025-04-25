using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using EmailCore;
using EmailServiceApi.Models;
namespace EmailServiceApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmailController : ControllerBase
    {
        private readonly EmailService _emailSender;

        public EmailController(EmailService emailSender)
        {
            _emailSender = emailSender;
        }

        [HttpPost("send")]
        public async Task<IActionResult> SendTestEmail([FromBody] EmailRequest request)
        {
            if (string.IsNullOrEmpty(request.RecipientEmail))
            {
                return BadRequest("Recipient email is required.");
            }

           var result = await _emailSender.SendEmailAsync(request.RecipientEmail, "Test Email from API", "This is a test email sent from the API.");
            if (result)
                return Ok("Email send.");
            else
                return BadRequest("Email Not sent");
        }
    }
}
