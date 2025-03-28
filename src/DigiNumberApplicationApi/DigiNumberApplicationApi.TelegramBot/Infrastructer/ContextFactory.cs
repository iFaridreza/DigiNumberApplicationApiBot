using DigiNumberApplicationApi.TelegramBot.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Data.Sqlite;

namespace DigiNumberApplicationApi.TelegramBot.Infrastructer;

public class ContextFactory : IDesignTimeDbContextFactory<Context>
{
    public Context CreateDbContext(string[] args)
    {
        AppSettings appSettings = Utils.GetAppSettings();

        var optionsBuilder = new DbContextOptionsBuilder<Context>();

        string basePath = AppContext.BaseDirectory;

        string dbPath = Path.Combine(basePath, appSettings.DatabaseName);

        SqliteConnectionStringBuilder connectionStringBuilder = new()
        {
            DataSource = dbPath
        };

        optionsBuilder.UseSqlite(connectionStringBuilder.ConnectionString);

        return new Context(optionsBuilder.Options);
    }
}