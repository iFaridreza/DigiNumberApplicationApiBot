namespace DigiNumberApplicationApi.TelegramBot.Core.Domian;

public class OrderVirtualNumber
{
    public long Id { get; set; }
    public string Date { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public long UserId { get; set; }
    public required User User { get; set; }
    public long VirtualNumberId { get; set; }
    public required VirtualNumber VirtualNumber { get; set; }
}