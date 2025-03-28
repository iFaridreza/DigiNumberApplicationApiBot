using System.Text.Json.Serialization;

namespace DigiNumberApplicationApi.TelegramBot.Api.Models;

public class RefreshToken : BaseResponse
{
    [JsonPropertyName("access_token")]
    public string AccsessToken { get; set; } = string.Empty;
}