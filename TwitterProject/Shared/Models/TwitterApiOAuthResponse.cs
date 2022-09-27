using System.Text.Json;
using System.Text.Json.Serialization;

namespace TwitterProject.Shared.Models
{
    public class TwitterApiOAuthResponse
    {
        [JsonPropertyName("token_type")]
        public string TokenType { get; set; }
        [JsonPropertyName("access_token")]
        public string AccessToken { get; set; }
    }
    public class TwitterApiLanguageResponse
    {
        [JsonPropertyName("code")]
        public string Code { get; set; }
        [JsonPropertyName("name")]
        public string Language { get; set; }
    }
}
