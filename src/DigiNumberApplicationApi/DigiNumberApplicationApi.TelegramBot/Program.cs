using DigiNumberApplicationApi.TelegramBot.Common;
using DigiNumberApplicationApi.TelegramBot.Core.IRepository;
using DigiNumberApplicationApi.TelegramBot.Infrastructer;
using DigiNumberApplicationApi.TelegramBot.StepUser;
using DigiNumberApplicationApi.TelegramBot.TelegramBot;
using Microsoft.Extensions.DependencyInjection;

AppSettings appSettings = Utils.GetAppSettings();

Utils.CreateSessionDir(appSettings.SessionPath);

Services.AddSqliteContext(appSettings.DatabaseName);
Services.AddStepUserContext(appSettings.SchedulerDatabaseName);
Services.AddILoggerTelegram(appSettings.TokenBot, appSettings.ChatIdLog);
Services.AddTelegramBotApi(appSettings.TokenBot);
Services.AddIRepositoryTransints();
Services.AddSchedulerBackground();

IServiceProvider serviceProvider = Services.Build();

Context context = serviceProvider.GetRequiredService<Context>();
context.Database.EnsureCreated();

SchedulerContext schedulerContext = serviceProvider.GetRequiredService<SchedulerContext>();
schedulerContext.Database.EnsureCreated();

ISudoRepository sudoRepository = serviceProvider.GetRequiredService<ISudoRepository>();
IVirtualSessionDetailsRepository virtualSessionDetailsRepository = serviceProvider.GetRequiredService<IVirtualSessionDetailsRepository>();

foreach (long sudo in appSettings.Sudos)
{
    bool anySudo = await sudoRepository.Any(sudo);

    if (anySudo)
    {
        continue;
    }

    await sudoRepository.Add(new()
    {
        ChatId = sudo
    });
}

if (!string.IsNullOrEmpty(appSettings.ApiHash) &&
    !string.IsNullOrEmpty(appSettings.ApiId) &&
    !string.IsNullOrEmpty(appSettings.Password2Fa))
{
    bool anySessionDetails = await virtualSessionDetailsRepository.Any();
    if (!anySessionDetails)
    {
        await virtualSessionDetailsRepository.Add(new()
        {
            ApiHash = appSettings.ApiHash,
            ApiId = appSettings.ApiId,
            Password2Fa = appSettings.Password2Fa,
        });
    }
}

BotApi botApi = serviceProvider.GetRequiredService<BotApi>();
SchedulerBackground schedulerBackground = serviceProvider.GetRequiredService<SchedulerBackground>();

botApi.Listen();
schedulerBackground.Start();

Console.ReadKey();