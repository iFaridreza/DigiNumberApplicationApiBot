using System.Text.Json.Serialization;

namespace DigiNumberApplicationApi.TelegramBot.Api.Models;

public class WalletUser : BaseResponse
{
    [JsonPropertyName("phonenumber")]
    public string PhoneNumber { get; set; } = string.Empty;

    [JsonPropertyName("wallet")]
    public string Wallet { get; set; } = string.Empty;
}