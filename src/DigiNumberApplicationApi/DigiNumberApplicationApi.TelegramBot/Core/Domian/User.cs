namespace DigiNumberApplicationApi.TelegramBot.Core.Domian;

public class User
{
    public long Id { get; set; }
    public long ChatId { get; set; }
    public string PhoneNumber { get; set; } = string.Empty;
    public bool IsVerify { get; set; }
}
