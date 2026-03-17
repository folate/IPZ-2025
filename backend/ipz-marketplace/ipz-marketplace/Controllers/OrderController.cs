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
        private readonly OrderTransactionService _orderTransactionService;
        private readonly IConfiguration _conf;
        public OrderController(MarketplaceDbContext context, 
            UserManager<User> userManager, 
            OrderService orderService, 
            OrderTransactionService orderTransactionService,
            IConfiguration conf)
        {
            _context = context;
            _userManager = userManager;
            _orderService = orderService;
            _orderTransactionService = orderTransactionService;
            _conf = conf;
        }

        [Authorize(Roles = "Buyer")]
        [HttpPost("create")]
        public async Task<IActionResult> GetAccessToken(OrderCreateDTO order)
        {
            var user = await _userManager.GetUserAsync(User);
            if(user == null)
            {
                return BadRequest("User not found!");
            }

            var merchantId = _conf["PaymentSettings:MerchantPosId"];
            if (string.IsNullOrWhiteSpace(merchantId))
            {
                return NotFound("Merchant Id problem");
            }

            var token = await _orderService.GetAccessToken();
            var client = new HttpClient();
            var actionResult = await _orderTransactionService.createOrder(order, user);
            var newOrder = new Order();

            if (actionResult is OkObjectResult okResult)
            {
                newOrder = okResult.Value as Order;
                if (newOrder == null)
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
                merchantPosId = merchantId,
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
