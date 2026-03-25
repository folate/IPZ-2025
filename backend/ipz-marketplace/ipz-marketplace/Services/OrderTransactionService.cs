using ipz_marketplace.DTOs;
using ipz_marketplace.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;

namespace ipz_marketplace.Services
{
    public class OrderTransactionService
    {
        public enum OrderStatus
        {
            New,
            Pending,
            Paid,
            Cancelled,
            Failed,
            Refunded,
            Completed
        }

        private readonly MarketplaceDbContext _context;
        public OrderTransactionService(MarketplaceDbContext context) 
        {
            _context = context;
        }
        public async Task<Order?> createOrder(OrderCreateDTO order, string userId, string extOrderId)
        {
            if (string.IsNullOrWhiteSpace(userId))
            {
                return null;
            }

            var buyers = await _context.Buyers.Where(u => u.UserId == userId).ToListAsync();
            Console.WriteLine($"\n\n\nDEBUG: User Identity ID: {userId} | Matched Buyers: {buyers.Count} | First Buyer ID: {buyers.FirstOrDefault()?.Id}\n\n\n");

            if (buyers.Count == 0)
            {
                return null;
            }

            if (buyers.Count > 1)
            {
                Console.WriteLine($"ERROR: Multiple buyer records found for UserId={userId}");
                return null;
            }

            var buyer = buyers[0];

            var gig = _context.Gigs.FirstOrDefault(g => g.Id == order.GigId);
            if (gig == null)
            {
                return null;
            }

            var gigs = await _context.Gigs
            .Include(g => g.SellerAd)
                .ThenInclude(sa => sa.Freelancer)
                    .ThenInclude(u => u.Seller)
            .FirstOrDefaultAsync(g => g.Id == order.GigId);
            if (gigs == null) throw new Exception("Gig nie istnieje w bazie");

            if (gigs.SellerAd == null)
                throw new Exception("Gig nie ma przypisanego SellerAd (sprawdź klucz obcy SellerAdId)");

            if (gigs.SellerAd.Freelancer == null)
                throw new Exception("SellerAd nie ma przypisanego Freelancera (sprawdź FreelancerId)");

            if (gigs.SellerAd.Freelancer.Seller == null)
                throw new Exception("Ten użytkownik nie ma jeszcze rekordu w tabeli Sellers!");

            var seller = gigs.SellerAd.Freelancer.Seller;

            var newOrder = new Order
            {
                ExtOrderId = extOrderId,
                Quantity = order.Quantity,
                Price = (int)order.Quantity * (int)gig.Price,
                AdditionalInstructions = order.AdditionalInstructions,
                AproxDeliveryDate = order.AproxDeliveryTime,
                OrderDate = DateTime.UtcNow,
                Status = OrderStatus.Pending.ToString(),
                GigsId = order.GigId,
                Gigs = gig,
                BuyerId = buyer.Id,
                Buyer = buyer,
                SellerId = seller.Id,
                Seller = seller
            };

            _context.Orders.Add(newOrder);
            buyer.TotalOrders += 1;
            _context.Buyers.Update(buyer);
            await _context.SaveChangesAsync();
            return newOrder;
        }

        public async Task<IActionResult> markOrderAs(string extOrderId, string mark)
        {
            var order = await _context.Orders.FirstOrDefaultAsync(o => o.ExtOrderId == extOrderId);
            if (order == null)
            {
                return new BadRequestObjectResult("Zamówienie nie znalezione");
            }

            order.Status = mark;
            order.OrderUpdateDate = DateTime.UtcNow;
            _context.Orders.Update(order);
            await _context.SaveChangesAsync();
            return new OkObjectResult($"Zamówienie oznaczone jako {mark}");
        }
    }

}
