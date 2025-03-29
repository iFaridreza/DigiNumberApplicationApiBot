namespace DigiNumberApplicationApi.TelegramBot.Api.Models;
public class WalletInventory : BaseResponse
{
    public bool CheckPrice() => Message == "ok" ? true : false;
}
