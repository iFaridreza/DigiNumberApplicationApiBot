namespace DigiNumberApplicationApi.TelegramBot.Core.Domian;

public class VirtualSessionDetails
{
    public long Id { get; set; }
    public string ApiId { get; set; } = string.Empty;
    public string ApiHash { get; set; } = string.Empty;
    public string Password2Fa { get; set; } = string.Empty;
}