using System.Text.RegularExpressions;
using Microsoft.Extensions.Configuration;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace DigiNumberApplicationApi.TelegramBot.Common;

public static class Utils
{
    public static void CreateSessionDir(string sessionPath)
    {
        string dirSessionPath = Path.Combine(AppContext.BaseDirectory, sessionPath);

        if (Directory.Exists(dirSessionPath))
        {
            return;
        }

        Directory.CreateDirectory(dirSessionPath);
    }

    public static AppSettings GetAppSettings()
    {
        const string pathJson = "appsettings.json";

        IConfiguration configuration = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile(pathJson)
            .Build();

        AppSettings appSettings = new();

        configuration.Bind(appSettings);

        return appSettings;
    }

    public static bool IsFlagCountry(string flagCountry)
    {
        if (string.IsNullOrEmpty(flagCountry) || flagCountry.Length != 4) return false;

        var utf32Chars = flagCountry.EnumerateRunes().ToArray();
        return utf32Chars.Length == 2 &&
               utf32Chars.All(rune => rune.Value is >= 0x1F1E6 and <= 0x1F1FF);
    }

    public static bool IsValidPhone(string phone)
    {
        string pattern = @"^09\d{9}$";
        bool isValid = Regex.IsMatch(phone, pattern);
        return isValid;
    }
}
