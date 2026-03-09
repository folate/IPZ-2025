using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Mvc;
using MimeKit;


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
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("Marketplace", "ipzmarketplace@onet.pl"));
            message.To.Add(new MailboxAddress("Odbiorca", "pe55848@zut.edu.pl"));
            message.Subject = "Dane konfiguracyjne działają!";
            message.Body = new TextPart("plain") { Text = "Testowa wiadomość." };

            using var client = new SmtpClient();
            try
            {
                await client.ConnectAsync("smtp.poczta.onet.pl", 465, SecureSocketOptions.SslOnConnect);
                client.AuthenticationMechanisms.Remove("XOAUTH2");
                client.AuthenticationMechanisms.Remove("NTLM");
                await client.AuthenticateAsync("ipzmarketplace@onet.pl", "PFUW-RO39-ZNRZ-0HXI");

                await client.SendAsync(message);
                await client.DisconnectAsync(true);

                Console.WriteLine("Sukces!");
            }
            catch (Exception ex) {
                return BadRequest($"Failed to send email: {ex.Message}");
            }

            return Ok("Email sent successfully");
        }
    }
}
