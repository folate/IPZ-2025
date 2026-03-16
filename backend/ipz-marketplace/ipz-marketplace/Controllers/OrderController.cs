using ipz_marketplace.DTOs;
using ipz_marketplace.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ipz_marketplace.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly MarketplaceDbContext _context;
        private readonly UserManager<User> _userManager;
        public OrderController(MarketplaceDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpGet("getToken")]
        public async Task<string> GetAccessToken()
        {
            var client = new HttpClient();
            var requestData = new Dictionary<string, string>
            {
                { "grant_type", "client_credentials" },
                { "client_id", "" },
                { "client_secret", "" }
            };
            var response = await client.PostAsync("https://secure.snd.payu.com/pl/standard/user/oauth/authorize", 
                new FormUrlEncodedContent(requestData));

            var json = await response.Content.ReadAsStringAsync();
            return json;
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

            var newOrder = new Order
            {
                Quantity = order.Quantity,
                Price = (int)order.Quantity * (int)_context.Gigs.FirstOrDefault(z => z.Id == order.GigId).Price,
                AdditionalInstructions = order.AdditionalInstructions,
                AproxDeliveryDate = order.AproxDeliveryTime,
                OrderDate = DateTime.Now,
                Status = "Pending",
                GigsId = order.GigId,
                Gigs = _context.Gigs.FirstOrDefault(g => g.Id == order.GigId),
                BuyerId = buyer.Id,
                Buyer = buyer,
                SellerId = order.SellerId,
                Seller = _context.Sellers.FirstOrDefault(u => u.Id == order.SellerId)
            };

            _context.Orders.Add(newOrder);
            buyer.TotalOrders += 1;
            _context.Buyers.Update(buyer);
            await _context.SaveChangesAsync();
            return Ok("Created sucesfully " + newOrder);
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
