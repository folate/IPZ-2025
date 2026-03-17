using ipz_marketplace.DTOs;
using ipz_marketplace.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;

namespace ipz_marketplace.Services
{
    public class OrderTransactionService
    {
        private readonly MarketplaceDbContext _context;
        public OrderTransactionService(MarketplaceDbContext context) 
        {
            _context = context;
        }
        public async Task<IActionResult> createOrder(OrderCreateDTO order, User user)
        {
            var buyer = _context.Buyers.FirstOrDefault(u => u.UserId == user.Id);

            if (buyer == null)
            {
                return new BadRequestObjectResult("Buyer not found");
            }

            var gig = _context.Gigs.FirstOrDefault(g => g.Id == order.GigId);
            if (gig == null)
            {
                return new BadRequestObjectResult("Gig not found");
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
                Quantity = order.Quantity,
                Price = (int)order.Quantity * (int)gig.Price,
                AdditionalInstructions = order.AdditionalInstructions,
                AproxDeliveryDate = order.AproxDeliveryTime,
                OrderDate = DateTime.UtcNow,
                Status = "Pending",
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
            return new OkObjectResult(newOrder);
        }
    }
}
