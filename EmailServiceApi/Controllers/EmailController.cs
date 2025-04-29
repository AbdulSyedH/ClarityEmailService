using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using EmailCore;
using EmailServiceApi.Models;
using EmailServiceApi.Utilities;
namespace EmailServiceApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmailController : ControllerBase
    {
        private readonly EmailService _emailSender;
        private readonly ILogger<EmailController> _logger;
        public EmailController(ILogger<EmailController> logger, EmailService emailSender)
        {
            _emailSender = emailSender;
            _logger = logger;
        }

        [HttpPost("send")]
        public async Task<IActionResult> SendTestEmail([FromBody] EmailRequest request)
        {
            if (string.IsNullOrEmpty(request.RecipientEmail))
            {
                _logger.LogWarning("Recipient email is required.");
                return BadRequest("Recipient email is required.");
            }
            if (!RegexUtilities.IsValidEmail(request.RecipientEmail))
            {
                _logger.LogWarning("email is not in write format.");
                return BadRequest("email is not in write format.");
            }

           var result = await _emailSender.SendEmailAsync(request.RecipientEmail, "Test Email from API", "This is a test email sent from the API.");
            if (result)
            {
                _logger.LogInformation("Email send.");
                return Ok("Email send.");
            }
            else
            {
                _logger.LogWarning("Email Not sent");
                return BadRequest("Email Not sent");
            }
            
        }
    }
}
