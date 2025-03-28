namespace DigiNumberApplicationApi.TelegramBot.Common;
public class AppSettings
{
    public string UsernameBot { get; set; } = string.Empty;
    public string TokenBot { get; set; } = string.Empty;
    public string BaseApiUrl { get; set; } = string.Empty;
    public string PasswordApi { get; set; } = string.Empty;
    public string ChatIdLog { get; set; } = string.Empty;
    public string UsernameOrderLog { get; set; } = string.Empty;
    public string DatabaseName { get; set; } = string.Empty;
    public string SchedulerDatabaseName { get; set; } = string.Empty;
    public string ApiId { get; set; } = string.Empty;
    public string ApiHash { get; set; } = string.Empty;
    public int TimeOutMinute { get; set; } 
    public string Password2Fa { get; set; } = string.Empty;
    public string SessionPath { get; set; } = string.Empty;
    public string UsernameSupport { get; set; } = string.Empty;
    public long[] Sudos { get; set; } = [];
    public Dictionary<string, string> ForceJoin { get; set; } = new();
}
