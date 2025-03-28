using DigiNumberApplicationApi.TelegramBot.Common;
using DigiNumberApplicationApi.TelegramBot.StepUser;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot;
using TimerSchadule = System.Timers.Timer;

public class SchedulerBackground
{
    private readonly TelegramBotClient _telegramBotClient;
    private readonly TimerSchadule _timerSchaduel;
    private readonly AppSettings _appSettings;
    private readonly IServiceProvider _serviceProvider;

    public SchedulerBackground(IServiceProvider serviceProvider, TelegramBotClient telegramBotClient)
    {
        _serviceProvider = serviceProvider;
        _telegramBotClient = telegramBotClient;
        _appSettings = Utils.GetAppSettings();
        _timerSchaduel = new();
    }

    internal void Start(int minuteInterval = 1)
    {
        _timerSchaduel.Interval = TimeSpan.FromMinutes(minuteInterval).TotalMilliseconds;
        _timerSchaduel.AutoReset = true;
        _timerSchaduel.Elapsed += _timerSchaduel_Elapsed;
        _timerSchaduel.Start();
    }

    private async void _timerSchaduel_Elapsed(object? sender, System.Timers.ElapsedEventArgs e)
    {
        IUserStepRepository userStepRepository = _serviceProvider.GetRequiredService<IUserStepRepository>();

        IEnumerable<UserStep> userSessions = await userStepRepository.GetAll();
        DateTime dateTimeNow = DateTime.Now;
        IEnumerable<UserStep> userSessionsExpier = userSessions.Where(x => x.ExpierTime < dateTimeNow).ToList();

        foreach (var item in userSessionsExpier)
        {
            try
            {
                await userStepRepository.Remove(item);

                //CashManager.Remove(item.ChatId);
                //await _telegramBotClient.SendMessage(item.ChatId, string.Format(ReplyText._timeOut, _appSettings.SchedulerTimeOut), ParseMode.Html);
            }
            catch { }
        }
    }
}
