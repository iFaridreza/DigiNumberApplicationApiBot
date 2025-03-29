using System.Text.Json;
using DigiNumberApplicationApi.TelegramBot.Api.Models;
using DigiNumberApplicationApi.TelegramBot.Common;

namespace DigiNumberApplicationApi.TelegramBot.Api;
public static class ApplicationApi
{
    private static string _baseApiUrl;
    private static string _passwordAuth;

    static ApplicationApi()
    {
        AppSettings appSettings = Utils.GetAppSettings();
        _baseApiUrl = appSettings.BaseApiUrl;
        _passwordAuth = appSettings.PasswordApi;
    }

    private static async Task<RefreshToken> RefreshTokenAsync()
    {
        using HttpClient client = new HttpClient();

        Dictionary<string, string> queryParams = new()
        {
            { "grant_type", "refresh_token" },
            { "client_secret", _passwordAuth }
        };

        string queryString = string.Join("&", queryParams.Select(x => $"{x.Key}={Uri.EscapeDataString(x.Value)}"));

        string url = $"{_baseApiUrl}?{queryString}";

        using HttpResponseMessage response = await client.GetAsync(url);

        response.EnsureSuccessStatusCode();

        string body = await response.Content.ReadAsStringAsync();

        string responseString = await response.Content.ReadAsStringAsync();

        RefreshToken? refreshToken = JsonSerializer.Deserialize<RefreshToken>(responseString);

        return refreshToken ?? throw new NullReferenceException(nameof(responseString));
    }

    public static async Task<bool> VerifyUserAsync(long chatId, string phone)
    {
        RefreshToken refreshToken = await RefreshTokenAsync();

        using HttpClient client = new HttpClient();

        Dictionary<string, string> queryParams = new()
        {
            { "grant_type", "check_user" },
            { "access_token", refreshToken.AccsessToken },
            { "user_token", chatId.ToString() },
            { "user_phone", phone },
        };

        string queryString = string.Join("&", queryParams.Select(x => $"{x.Key}={Uri.EscapeDataString(x.Value)}"));

        string url = $"{_baseApiUrl}?{queryString}";

        using HttpResponseMessage response = await client.GetAsync(url);

        response.EnsureSuccessStatusCode();

        string body = await response.Content.ReadAsStringAsync();

        string responseString = await response.Content.ReadAsStringAsync();

        VerifyUser? verifyUser = JsonSerializer.Deserialize<VerifyUser>(responseString);

        return verifyUser is null ? false : verifyUser.IsVerify();
    }

    public static async Task<RegisterUser> RegisterUserOTPAsync(long chatId, string phone)
    {
        RefreshToken refreshToken = await RefreshTokenAsync();

        using HttpClient client = new HttpClient();

        Dictionary<string, string> queryParams = new()
        {
            { "grant_type", "register" },
            { "access_token", refreshToken.AccsessToken },
            { "user_token", chatId.ToString() },
            { "user_phone", phone },
        };

        string queryString = string.Join("&", queryParams.Select(x => $"{x.Key}={Uri.EscapeDataString(x.Value)}"));

        string url = $"{_baseApiUrl}?{queryString}";

        using HttpResponseMessage response = await client.GetAsync(url);

        response.EnsureSuccessStatusCode();

        string body = await response.Content.ReadAsStringAsync();

        string responseString = await response.Content.ReadAsStringAsync();

        RegisterUser? registerUser = JsonSerializer.Deserialize<RegisterUser>(responseString);

        return registerUser == null ? throw new NullReferenceException(nameof(registerUser)) : string.IsNullOrEmpty(registerUser.CodeToken) ? throw new Exception("User Exist") : registerUser;
    }

    public static async Task<bool> VerifyUserOTPAsync(long chatId, string phone, string code, string codeToken)
    {
        RefreshToken refreshToken = await RefreshTokenAsync();

        using HttpClient client = new HttpClient();

        Dictionary<string, string> queryParams = new()
        {
            { "grant_type", "user_verification" },
            { "access_token", refreshToken.AccsessToken },
            { "user_token", chatId.ToString() },
            { "user_phone", phone },
            { "code", code },
            { "code_token", codeToken }
        };

        string queryString = string.Join("&", queryParams.Select(x => $"{x.Key}={Uri.EscapeDataString(x.Value)}"));

        string url = $"{_baseApiUrl}?{queryString}";

        using HttpResponseMessage response = await client.GetAsync(url);

        response.EnsureSuccessStatusCode();

        string body = await response.Content.ReadAsStringAsync();

        string responseString = await response.Content.ReadAsStringAsync();

        VerifyOTPUser? verifyOTPUser = JsonSerializer.Deserialize<VerifyOTPUser>(responseString);

        return verifyOTPUser == null ? false : verifyOTPUser.IsOtpVerify();
    }

    public static async Task<WalletUser> WalletUserAsync(long chatId)
    {
        RefreshToken refreshToken = await RefreshTokenAsync();

        using HttpClient client = new HttpClient();

        Dictionary<string, string> queryParams = new()
        {
            { "grant_type", "getwallet" },
            { "access_token", refreshToken.AccsessToken },
            { "user_token", chatId.ToString() }
        };

        string queryString = string.Join("&", queryParams.Select(x => $"{x.Key}={Uri.EscapeDataString(x.Value)}"));

        string url = $"{_baseApiUrl}?{queryString}";

        using HttpResponseMessage response = await client.GetAsync(url);

        response.EnsureSuccessStatusCode();

        string body = await response.Content.ReadAsStringAsync();

        string responseString = await response.Content.ReadAsStringAsync();

        WalletUser? walletUser = JsonSerializer.Deserialize<WalletUser>(responseString);

        return walletUser == null ? throw new NullReferenceException(nameof(walletUser)) : walletUser;
    }

    public static async Task<bool> WalletInventoryAsync(long chatId, decimal price)
    {

        RefreshToken refreshToken = await RefreshTokenAsync();

        using HttpClient client = new HttpClient();

        Dictionary<string, string> queryParams = new()
        {
            { "grant_type", "check_wallet" },
            { "access_token", refreshToken.AccsessToken },
            { "user_token", chatId.ToString() },
            {"price" ,price.ToString() }
        };

        string queryString = string.Join("&", queryParams.Select(x => $"{x.Key}={Uri.EscapeDataString(x.Value)}"));

        string url = $"{_baseApiUrl}?{queryString}";

        using HttpResponseMessage response = await client.GetAsync(url);

        response.EnsureSuccessStatusCode();

        string body = await response.Content.ReadAsStringAsync();

        string responseString = await response.Content.ReadAsStringAsync();

        WalletInventory? walletInventory = JsonSerializer.Deserialize<WalletInventory>(responseString);

        return walletInventory == null ? false : walletInventory.CheckPrice();
    }

    public static async Task<RedirectPayment> RedirectPaymentAsync(long chatId, decimal price)
    {
        RefreshToken refreshToken = await RefreshTokenAsync();

        using HttpClient client = new HttpClient();

        Dictionary<string, string> queryParams = new()
        {
            { "grant_type", "payment" },
            { "access_token", refreshToken.AccsessToken },
            { "user_token", chatId.ToString() },
            {"price" ,price.ToString() }
        };

        string queryString = string.Join("&", queryParams.Select(x => $"{x.Key}={Uri.EscapeDataString(x.Value)}"));

        string url = $"{_baseApiUrl}?{queryString}";

        using HttpResponseMessage response = await client.GetAsync(url);

        response.EnsureSuccessStatusCode();

        string body = await response.Content.ReadAsStringAsync();

        string responseString = await response.Content.ReadAsStringAsync();

        RedirectPayment? redirectPayment = JsonSerializer.Deserialize<RedirectPayment>(responseString);

        return redirectPayment == null ? throw new NullReferenceException(nameof(RedirectPayment)) : redirectPayment;
    }
    
    public static async Task<bool> WalletWitdhra(long chatId,decimal price)
    {
        RefreshToken refreshToken = await RefreshTokenAsync();

        using HttpClient client = new HttpClient();

        Dictionary<string, string> queryParams = new()
        {
            { "grant_type", "deduct_wallet" },
            { "access_token", refreshToken.AccsessToken },
            { "user_token", chatId.ToString() },
            {"price" ,price.ToString() }
        };

        string queryString = string.Join("&", queryParams.Select(x => $"{x.Key}={Uri.EscapeDataString(x.Value)}"));

        string url = $"{_baseApiUrl}?{queryString}";

        using HttpResponseMessage response = await client.GetAsync(url);

        response.EnsureSuccessStatusCode();

        string body = await response.Content.ReadAsStringAsync();

        string responseString = await response.Content.ReadAsStringAsync();

        WalletWidthra? walletWidthra = JsonSerializer.Deserialize<WalletWidthra>(responseString);

        return walletWidthra == null ? false : walletWidthra.IsSucsessWidhra();
    }
}