namespace DigiNumberApplicationApi.TelegramBot.Api.Models;
public class WalletInventory : BaseResponse
{
    public bool CheckPrice(decimal price) => Message == "ok" ? true : false;
}
