using DigiNumberApplicationApi.TelegramBot.Infrastructer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Data.Sqlite;
using DigiNumberApplicationApi.TelegramBot.Core.IRepository;
using DigiNumberApplicationApi.TelegramBot.Infrastructer.Repository;
using Serilog;
using Serilog.Sinks.TelegramBot;
using Telegram.Bot;
using DigiNumberApplicationApi.TelegramBot.TelegramBot;
using DigiNumberApplicationApi.TelegramBot.StepUser;

namespace DigiNumberApplicationApi.TelegramBot.Common;

public static class Services
{
    private static IServiceCollection _serviceCollection;

    static Services() => _serviceCollection = new ServiceCollection();

    public static void AddSqliteContext(string databaseName)
    {
        string basePath = AppContext.BaseDirectory;

        string dbPath = Path.Combine(basePath, databaseName);

        SqliteConnectionStringBuilder connectionStringBuilder = new()
        {
            DataSource = dbPath
        };

        _serviceCollection.AddDbContext<Context>(x => x.UseSqlite(connectionStringBuilder.ConnectionString));
    }

    public static void AddIRepositoryTransints()
    {
        _serviceCollection.AddTransient<ISudoRepository, SudoRepository>();
        _serviceCollection.AddTransient<IUserRepository, UserRepository>();
        _serviceCollection.AddTransient<IOrderVirtualNumberRepository, OrderVirtualNumberRepository>();
        _serviceCollection.AddTransient<IVirtualNumberRepository, VirtualNumberRepository>();
        _serviceCollection.AddTransient<IVirtualNumberDetailsRepository, VirtualNumberDetailsRepository>();
        _serviceCollection.AddTransient<IVirtualSessionDetailsRepository, VirtualSessionDetailsRepository>();
        _serviceCollection.AddTransient<IUserStepRepository, UserStepRepository>();
    }

    public static void AddTelegramBotApi(string token)
    {
        _serviceCollection.AddSingleton(x =>
        {
            return new TelegramBotClient(token);
        });

        _serviceCollection.AddSingleton<BotApi>();
    }

    public static void AddStepUserContext(string databaseName)
    {
        string basePath = AppContext.BaseDirectory;

        string dbPath = Path.Combine(basePath, databaseName);

        SqliteConnectionStringBuilder connectionStringBuilder = new()
        {
            DataSource = dbPath
        };

        _serviceCollection.AddDbContext<SchedulerContext>(x => x.UseSqlite(connectionStringBuilder.ConnectionString));
    }

    public static void AddILoggerTelegram(string token,string chatId)
    {
        _serviceCollection.AddSingleton<ILogger>(x =>
        {
            return new LoggerConfiguration().MinimumLevel.Verbose().WriteTo.TelegramBot(token, chatId).CreateLogger();
        });
    }

    public static void AddSchedulerBackground()
    {
        _serviceCollection.AddSingleton<SchedulerBackground>();
    }

    public static IServiceProvider Build() => _serviceCollection.BuildServiceProvider();
}
