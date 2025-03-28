namespace DigiNumberApplicationApi.TelegramBot.Cashe;

public class CashSession
{
    public IDictionary<string, string> keyValuePairs { get; init; }

    public CashSession()
    {
        keyValuePairs = new Dictionary<string, string>();
    }
}
