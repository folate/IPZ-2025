using ipz_marketplace.DTOs;
using ipz_marketplace.Entities;
using ipz_marketplace.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using static ipz_marketplace.Controllers.OrderController;

namespace ipz_marketplace.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class OrderController : ControllerBase
    {
        public enum OrderStatus
        {
            New,
            Pending,
            Paid,
            Cancelled,
            Failed,
            Refunded
        }
        public class PayUOrderResponse
        {
            [JsonPropertyName("status")]
            public PayUStatus Status { get; set; }

            [JsonPropertyName("redirectUri")]
            public string RedirectUri { get; set; }

            [JsonPropertyName("orderId")]
            public string OrderId { get; set; }

            [JsonPropertyName("extOrderId")]
            public string ExtOrderId { get; set; }
        }

        public class PayUStatus
        {
            [JsonPropertyName("statusCode")]
            public string StatusCode { get; set; }
        }

        private readonly MarketplaceDbContext _context;
        private readonly UserManager<User> _userManager;
        private readonly OrderService _orderService;
        private readonly OrderTransactionService _orderTransactionService;
        private readonly IConfiguration _conf;
        private readonly EmailService _emailService;
        public OrderController(MarketplaceDbContext context, 
            UserManager<User> userManager, 
            OrderService orderService, 
            OrderTransactionService orderTransactionService,
            IConfiguration conf,
            EmailService emailService)
        {
            _context = context;
            _userManager = userManager;
            _orderService = orderService;
            _orderTransactionService = orderTransactionService;
            _conf = conf;
            _emailService = emailService;
        }

        [Authorize(Roles = "Buyer")]
        [HttpPost("create")]
        public async Task<IActionResult> GetAccessToken(OrderCreateDTO order)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return BadRequest("User not found!");
            }

            var merchantId = _conf["PaymentSettings:MerchantPosId"];
            if (string.IsNullOrWhiteSpace(merchantId))
            {
                return NotFound("Merchant Id problem");
            }

            var token = await _orderService.GetAccessToken();
            var handler = new HttpClientHandler
            {
                AllowAutoRedirect = false
            };
            var client = new HttpClient(handler);

            var extOrder = Guid.NewGuid().ToString();
            var newOrder = await _orderTransactionService.createOrder(order, user.Id, extOrder);
            if(newOrder == null)
            {
                return BadRequest("Problem with creating order");
            }

            string amountInGrosze = ((int)(newOrder.Price * 100)).ToString();

            var payuOrder = new
            {
                notifyUrl = "https://shyla-pedagoguish-beamishly.ngrok-free.dev/api/Order/notify",
                continueUrl = "https://localhost/thanks",
                customerIp = "127.0.0.1",
                merchantPosId = merchantId,
                description = $"Payment for order by Id: {newOrder.Id}",
                currencyCode = "PLN",
                totalAmount = amountInGrosze,
                extOrderId = extOrder,
                products = new[]
                {
                    new { 
                        name = $"Gig id: {order.GigId}",
                        unitPrice = amountInGrosze, 
                        quantity = "1" 
                    }
                }
            };


            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var response = await client.PostAsJsonAsync("https://secure.snd.payu.com/api/v2_1/orders",payuOrder);
            var content = await response.Content.ReadAsStringAsync();

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            try
            {
                var result = JsonSerializer.Deserialize<PayUOrderResponse>(content, options);

                if (result?.Status?.StatusCode == "SUCCESS" || result?.Status?.StatusCode == "WARNING_CONTINUE_3DS")
                {
                    await _emailService.EmailConnection(user.Email, "Order Created", 
                        $"Your order with id {newOrder.Id} has been created.");
                    return Ok(new 
                    { 
                        url = result.RedirectUri 
                    });
                }

                return BadRequest($"PayU zwróciło status: {result?.Status?.StatusCode}");
            }
            catch (JsonException ex)
            {
                Console.WriteLine($"Błąd parsowania: {ex.Message}");
                Console.WriteLine($"Otrzymane body: {content}");
                return StatusCode(500, "Błąd komunikacji z PayU");
            }
        }

        [HttpPost("notify")]
        [AllowAnonymous]
        public async Task<IActionResult> Notify([FromBody] PayUNotificationDTO notification)
        {
            Console.WriteLine($"Otrzymano powiadomienie od PayU: {notification.Order.ExtOrderId}, ze statusem {notification.Order.Status}");

            if (notification?.Order == null)
            {
                return BadRequest("Invalid payload");
            }
            var extOrderId = notification.Order.ExtOrderId;

            switch (notification.Order.Status)
            {
                case "COMPLETED":
                    await _orderTransactionService.markOrderAs(extOrderId, OrderStatus.Paid.ToString());
                    break;

                case "PENDING":
                    await _orderTransactionService.markOrderAs(extOrderId, OrderStatus.Pending.ToString());
                    break;

                case "CANCELED":
                    await _orderTransactionService.markOrderAs(extOrderId, OrderStatus.Cancelled.ToString());
                    break;

                case "REJECTED":
                case "FAILED":
                    await _orderTransactionService.markOrderAs(extOrderId, OrderStatus.Failed.ToString());
                    break;

                default:
                    Console.WriteLine($"Inny status: {notification.Order.Status}");
                    break;
            }

            return Ok($"Order completed with {notification.Order.Status}");
            
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

            var orders = _context.Orders.Where(o => o.Buyer.UserId == userId).ToList();

            return Ok(orders);

        }
    }
}
