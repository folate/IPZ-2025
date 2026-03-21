using ipz_marketplace.DTOs;

namespace ipz_marketplace.Services
{
    public class OrderService
    {
        private readonly IConfiguration _config;
        public OrderService(IConfiguration config)
        {
            _config = config;
        }

        public async Task<string> GetAccessToken()
        {
            var clientId = _config["PaymentSettings:ClientId"];
            if(string.IsNullOrWhiteSpace(clientId))
            {
                throw new InvalidOperationException("PaymentSettings:ClientId is not configured.");
            }

            var clientSecret = _config["PaymentSettings:ClientSecret"];
            if (string.IsNullOrWhiteSpace(clientSecret))
            {
                throw new InvalidOperationException("PaymentSettings:ClientSecret is not configured.");
            }

            var client = new HttpClient();
            var requestData = new Dictionary<string, string>
            {
                { "grant_type", "client_credentials" },
                { "client_id", clientId },
                { "client_secret", clientSecret }
            };
            var response = await client.PostAsync("https://secure.snd.payu.com/pl/standard/user/oauth/authorize",
                new FormUrlEncodedContent(requestData));

            var json = await response.Content.ReadFromJsonAsync<PayUTokenResponseDTO>();

            if (json == null || string.IsNullOrEmpty(json.AccessToken))
            {
                throw new Exception("PayU zwróciło pusty token." + json);
            }

            return json.AccessToken;
        }
    }
}
