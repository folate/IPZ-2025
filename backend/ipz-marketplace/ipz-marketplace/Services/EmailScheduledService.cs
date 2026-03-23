
using Microsoft.EntityFrameworkCore;

namespace ipz_marketplace.Services
{
    public class EmailScheduledService
    {
        private readonly MarketplaceDbContext _context;
        private readonly EmailService _emailService;

        public EmailScheduledService(MarketplaceDbContext context, EmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }

        public async Task SendScheduledEmails(int userSize)
        {
            int pageNumber = 0;
            while (true)
            {
                var sellers = _context.Sellers
                    .Include(s => s.User)
                    .OrderBy(b => b.Id)
                    .Skip(userSize * pageNumber)
                    .Take(userSize)
                    .ToList();

                if (!sellers.Any()) break;

                foreach (var user in sellers)
                {
                    await _emailService.EmailConnection(user.User.Email, $"Scheduled Email for {user.User.UserName}",
                        $"This is scheduled email for you! Right now you have {user.CompletedJobs} completed jobs.");
                }
                pageNumber++;
            }
        }
    }
}
