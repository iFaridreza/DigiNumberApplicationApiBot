namespace DigiNumberApplicationApi.TelegramBot.Api.Models;
public class VerifyUser : BaseResponse
{
    public bool IsVerify() => Message == "user_found" ? true : false;
}
