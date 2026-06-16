using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace hotel_booking_api.DTOs.Auth
{
    public class GoogleLoginDto
    {
        [JsonPropertyName("idToken")]
        public string IdToken { get; set; } = string.Empty;
    }
}
