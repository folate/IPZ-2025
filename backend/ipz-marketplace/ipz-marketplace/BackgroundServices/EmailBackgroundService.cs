using ipz_marketplace.Services;
using Quartz;

namespace ipz_marketplace.BackgroundServices
{
    public class EmailBackgroundService : IJob
    {
        private readonly EmailScheduledService _emailScheduledService;
        private readonly ILogger<EmailBackgroundService> _logger;
        public EmailBackgroundService(EmailScheduledService emailScheduledService, ILogger<EmailBackgroundService> logger)
        {
            _emailScheduledService = emailScheduledService;
            _logger = logger;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            Console.WriteLine($"Executing EmailBackgroundService at {DateTimeOffset.Now}");
            _logger.LogInformation("Executing EmailBackgroundService at {Time}", DateTimeOffset.Now);
            await _emailScheduledService.SendScheduledEmails(10);
        }

    }
}
