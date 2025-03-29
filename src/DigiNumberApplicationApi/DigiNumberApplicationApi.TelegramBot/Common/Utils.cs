using System.Text.RegularExpressions;
using Microsoft.Extensions.Configuration;
using PhoneNumbers;

namespace DigiNumberApplicationApi.TelegramBot.Common;

public static class Utils
{
    public static void CreateSessionDir(string sessionPath)
    {
        string dirSessionPath = GetSessionsDirPath(sessionPath);

        if (Directory.Exists(dirSessionPath))
        {
            return;
        }

        Directory.CreateDirectory(dirSessionPath);
    }
    public static string? GetLoginCode( string lastMessage)
    {
        string pattern = @"\b\d{5}\b";

        Match match = Regex.Match(lastMessage, pattern);

        return match.Success ? match.Value : null;
    }
    public static string GetSessionsDirPath(string sessionPath)
    {
        string dirSessionPath = Path.Combine(AppContext.BaseDirectory, sessionPath);
        return dirSessionPath;
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

    public static InfoPhoneNumber InfoPhoneNumber(string phoneNumber)
    {
        PhoneNumberUtil phoneNumberInit = PhoneNumberUtil.GetInstance();

        PhoneNumber number = phoneNumberInit.Parse(phoneNumber, null);

        string regionCode = phoneNumberInit.GetRegionCodeForNumber(number);

        int regionCountry = phoneNumberInit.GetCountryCodeForRegion(regionCode);

        return new InfoPhoneNumber(regionCode, $"+{regionCountry}", number.NationalNumber.ToString());
    }
}