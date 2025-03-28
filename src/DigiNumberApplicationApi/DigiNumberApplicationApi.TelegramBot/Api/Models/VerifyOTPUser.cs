namespace DigiNumberApplicationApi.TelegramBot.Api.Models;

public class VerifyOTPUser : BaseResponse
{
    public bool IsOtpVerify() => Message == "code_ok" ? true : false;
}