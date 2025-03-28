using System.Text.Json.Serialization;

namespace DigiNumberApplicationApi.TelegramBot.Api.Models;
public class BaseResponse
{
    [JsonPropertyName("status")]
    public bool Status { get; set; }
    [JsonPropertyName("message")]
    public string Message { get; set; } = string.Empty;
}
