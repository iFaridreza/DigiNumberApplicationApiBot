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

   
    public static bool IsValidPhone(string phone)
    {
        string pattern = @"^09\d{9}$";
        bool isValid = Regex.IsMatch(phone, pattern);
        return isValid;
    }
}
