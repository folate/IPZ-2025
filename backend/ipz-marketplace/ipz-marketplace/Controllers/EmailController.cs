using MailKit.Net.Smtp;
using MimeKit;
using Microsoft.AspNetCore.Mvc;


namespace ipz_marketplace.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmailController : ControllerBase
    {
        public EmailController()
        {
        }

        [HttpPost]
        public async Task<IActionResult> SendEmail()
        {
            var email = new MimeMessage();
            var smtpClient = new SmtpClient();
            var builder = new BodyBuilder()
            {
                TextBody = @"This is a test email sent from the IPZ Marketplace application."
            };
            try
            {
                email.From.Add(new MailboxAddress("IPZ Marketplace", "noreply@ipzmarketplace.com"));
                email.To.Add(new MailboxAddress("Recipient", "pe55848@zut.edu.pl"));
                email.Subject = "Test Email from IPZ Marketplace";
                email.Body = builder.ToMessageBody();
                smtpClient.Connect("live.smtp.mailtrap.io", 587, false);

                smtpClient.Authenticate("api", "703681c4b6878019963803894ca30d88");
                smtpClient.Send(email);
                smtpClient.Disconnect(true);
            }
            catch (Exception ex) {
                return BadRequest($"Failed to send email: {ex.Message}");
            }

            return Ok("Email sent successfully");
        }
    }
}
