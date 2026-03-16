using ipz_marketplace.DTOs;
using ipz_marketplace.Entities;
using ipz_marketplace.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ipz_marketplace.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly MarketplaceDbContext _context;
        private readonly UserManager<User> _userManager;
        private readonly OrderService _orderService;
        public OrderController(MarketplaceDbContext context, UserManager<User> userManager, OrderService orderService)
        {
            _context = context;
            _userManager = userManager;
            _orderService = orderService;
        }

        
        public async Task<IActionResult> GetAccessToken(OrderCreateDTO order)
        {
            var token = await _orderService.GetAccessToken();
            var client = new HttpClient();
            var actionResult = await createOrder(order);
            var newOrder = new Order();

            if (actionResult is OkObjectResult okResult)
            {
                newOrder = okResult.Value as Order;
                if(newOrder == null)
                {
                    return BadRequest("Cannot create order");
                }
            }
            else
            {
                return BadRequest("Cannot create order");
            }
            
            var payuOrder = new
            {
                customerIp = "127.0.0.1",
                merchantPosId = newOrder.SellerId,
                description = $"Payment for order by Id: {newOrder.Id}",
                currencyCode = "PLN",
                totalAmount = newOrder.Price + "00",
                products = new[]
                {
                    new { name = $"Gig id: {newOrder.GigsId}", unitPrice = newOrder.Price + "00", quantity = "1" }
                }
            };
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            var response = await client.PostAsJsonAsync("https://secure.payu.com/api/v2_1/orders", order);

            return Ok(response);
        }

        [Authorize(Roles = "Buyer")]
        [HttpPut("create")]
        public async Task<IActionResult> createOrder([FromBody] OrderCreateDTO order)
        {
            var userId = _userManager.GetUserId(User);
            var buyer = _context.Buyers.FirstOrDefault(u => u.UserId == userId);

            if (buyer == null)
            {
                return BadRequest("Buyer not found");
            }

            var gig = _context.Gigs.FirstOrDefault(g => g.Id == order.GigId);
            if (gig == null) {
                return BadRequest("Gig not found");
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
            return Ok(newOrder);
        }

        [Authorize(Roles = "Buyer")]
        [HttpGet("myorders")]
        public async Task<IActionResult> GetMyOrders()
        {
            var userId = _userManager.GetUserId(User);
            var buyer = _context.Buyers.FirstOrDefault(u => u.UserId == userId);

            if (buyer == null)
            {
                return BadRequest("Buyer not found");
            }

            var orders = _context.Orders.Where(o => o.BuyerId == buyer.Id).ToList();
            return Ok(orders);

        }
    }
}
