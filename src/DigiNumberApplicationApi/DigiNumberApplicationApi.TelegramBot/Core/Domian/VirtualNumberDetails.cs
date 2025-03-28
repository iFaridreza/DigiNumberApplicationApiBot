namespace DigiNumberApplicationApi.TelegramBot.Core.Domian;

public class VirtualNumberDetails
{
    public long Id { get; set; }
    public string CountryCode { get; set; } = string.Empty;
    public string CountryName { get; set; } = string.Empty;
    public string Flag { get; set; } = string.Empty;
    public decimal Price { get; set; }
}