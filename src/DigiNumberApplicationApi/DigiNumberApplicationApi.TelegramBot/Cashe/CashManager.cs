using System.Collections.Concurrent;

namespace DigiNumberApplicationApi.TelegramBot.Cashe;

public static class CashManager
{
    private static ConcurrentDictionary<long, CashSession> keyValuePairs;

    static CashManager() => keyValuePairs = new();

    public static void AddOrUpdate(long chatId, CashSession cashSession) => keyValuePairs.AddOrUpdate(chatId, cashSession, (chatId, cash) => cashSession);

    public static bool Any(long chatId) => keyValuePairs.ContainsKey(chatId);

    public static CashSession? Get(long chatId)
    {
        keyValuePairs.TryGetValue(chatId, out CashSession? cashSession);

        return cashSession;
    }

    public static void Remove(long chatId) => keyValuePairs.TryRemove(chatId, out _);
}