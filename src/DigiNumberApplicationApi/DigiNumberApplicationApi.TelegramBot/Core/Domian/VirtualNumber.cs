namespace DigiNumberApplicationApi.TelegramBot.Core.Domian;

public class VirtualNumber
{
    public long Id { get; set; }
    public string CountryCode { get; set; } = string.Empty;
    public string Number { get; set; } = string.Empty;
    public EStatusOrder EStatusOrder { get; set; }
    public long VirtualSessionDetailsId { get; set; }
    public required VirtualSessionDetails VirtualSessionDetails { get; set; }
}
