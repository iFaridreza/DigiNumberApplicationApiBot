using TL;
using WTelegram;

namespace DigiNumberApplicationApi.TelegramBot.TelegramBot;
public class WTelegramClientManager
{
    private Client _client;
    private string? _number;
    public string ApiId { get; }
    public string ApiHash { get; }
    public string SessionPath { get; }
    public WTelegramClientManager(string apiId, string apiHash, string sessionPath)
    {
        ApiId = apiId;
        ApiHash = apiHash;
        SessionPath = sessionPath;
        _client = null!;
    }

    private string? Config(string what)
    {
        return what switch
        {
            "api_id" => ApiId,
            "api_hash" => ApiHash,
            "session_pathname" => Path.Combine(SessionPath, $"{_number}.session"),
            _ => null
        };
    }

    public async Task Connect(string number)
    {
        if (string.IsNullOrWhiteSpace(number)) throw new NullReferenceException(nameof(number));

        _number = number;

        _client = new(Config);

        await _client.ConnectAsync();
    }

    public async Task Disconnect()
    {
        await _client.DisposeAsync();
    }

    public async Task Logout()
    {
        await _client.Auth_LogOut();
    }

    public async Task<string> Login(string state)
    {
        string result = await _client.Login(state);
        return result;
    }

    public async Task ChangeOrDisablePassword2Fa(string currentPassword, string? newPassword)
    {
        Account_Password accountPwd = await _client.Account_GetPassword();
        InputCheckPasswordSRP? password = accountPwd.current_algo == null ? null : await Client.InputCheckPassword(accountPwd, currentPassword);
        accountPwd.current_algo = null;
        InputCheckPasswordSRP? new_password_hash = newPassword == null ? null : await Client.InputCheckPassword(accountPwd, newPassword);
        await _client.Account_UpdatePasswordSettings(password, new Account_PasswordInputSettings
        {
            flags = Account_PasswordInputSettings.Flags.has_new_algo,
            new_password_hash = new_password_hash?.A ?? null,
            new_algo = accountPwd.new_algo ?? null
        });
    }

    public async Task<int> ActiveSessionCount()
    {
        Account_Authorizations authorizations = await _client.Account_GetAuthorizations();
        int count = authorizations.authorizations.Count();
        return count;
    }

    public async Task<string[]> GetMessagesText(string contactPhone, int limit)
    {
        Contacts_ResolvedPeer inputPeerUser = await _client.Contacts_ResolvePhone(contactPhone);
        Messages_MessagesBase resultHistory = await _client.Messages_GetHistory(inputPeerUser, limit: limit);

        ICollection<string> messages = new List<string>();

        foreach (var item in resultHistory.Messages)
        {
            Message? message = item as Message;

            if (message is null) continue;

            messages.Add(message.message);
        }

        return messages.ToArray();
    }

    public void Loging(Action<int, string> loging)
    {
        Helpers.Log = loging;
    }

    public void DisableUpdate(bool disable = true)
    {
        _client.DisableUpdates(disable);
    }
}