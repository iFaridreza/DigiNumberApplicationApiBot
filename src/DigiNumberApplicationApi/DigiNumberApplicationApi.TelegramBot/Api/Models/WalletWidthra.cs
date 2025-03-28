namespace DigiNumberApplicationApi.TelegramBot.Api.Models;
public class WalletWidthra : BaseResponse
{
    public bool IsSucsessWidhra() => Message == "ok" ? true : false;
}
