using System.Text.Json.Serialization;

namespace ipz_marketplace.DTOs
{
    public class PayUTokenResponseDTO
    {
            [JsonPropertyName("access_token")]
            public string AccessToken { get; set; }

            [JsonPropertyName("token_type")]
            public string TokenType { get; set; }

            [JsonPropertyName("expires_in")]
            public int ExpiresIn { get; set; }

            [JsonPropertyName("grant_type")]
            public string GrantType { get; set; }
    }
}
