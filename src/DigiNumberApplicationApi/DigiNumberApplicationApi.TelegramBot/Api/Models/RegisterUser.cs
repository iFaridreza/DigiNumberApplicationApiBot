using System.Text.Json.Serialization;

namespace DigiNumberApplicationApi.TelegramBot.Api.Models;
public class RegisterUser : BaseResponse
{
    [JsonPropertyName("code_token")]
    public string CodeToken { get; set; } = string.Empty;
}
