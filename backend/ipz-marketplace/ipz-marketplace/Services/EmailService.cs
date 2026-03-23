using System;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using Microsoft.Extensions.Configuration;

namespace ipz_marketplace.Services
{
    public class EmailService
    {
        private readonly IConfiguration _config;
        public EmailService(IConfiguration config) 
        {
            _config = config;
        }

        public async Task EmailConnection(string to, string subject, string body) 
        {
            var message = new MimeMessage();

            var fromAddress = _config["EmailSettings:Email"];
            if (string.IsNullOrWhiteSpace(fromAddress))
                throw new InvalidOperationException("EmailSettings:Email is not configured.");

            message.From.Add(new MailboxAddress("IPZ Marketplace", fromAddress));
            message.To.Add(MailboxAddress.Parse(to));
            message.Subject = subject ?? string.Empty;

            var builder = new BodyBuilder
            {
                HtmlBody = body ?? string.Empty
            };
            message.Body = builder.ToMessageBody();

            var host = _config["EmailSettings:Host"];

            var password = _config["EmailSettings:Password"];
            if (string.IsNullOrWhiteSpace(password))
                throw new InvalidOperationException("EmailSettings:Password is not configured.");

            using var client = new SmtpClient();
            await client.ConnectAsync(host, 465, true);
            await client.AuthenticateAsync(fromAddress, password);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);
        }
    }
}
