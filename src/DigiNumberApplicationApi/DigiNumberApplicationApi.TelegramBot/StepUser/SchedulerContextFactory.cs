using DigiNumberApplicationApi.TelegramBot.Common;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace DigiNumberApplicationApi.TelegramBot.StepUser;

public class SchedulerContextFactory : IDesignTimeDbContextFactory<SchedulerContext>
{
    public SchedulerContext CreateDbContext(string[] args)
    {
        AppSettings appSettings = Utils.GetAppSettings();

        string basePath = AppContext.BaseDirectory;

        string dbPath = Path.Combine(basePath, appSettings.SchedulerDatabaseName);

        SqliteConnectionStringBuilder connectionStringBuilder = new()
        {
            DataSource = dbPath
        };
        var optionsBuilder = new DbContextOptionsBuilder<SchedulerContext>();

        optionsBuilder.UseSqlite(connectionStringBuilder.ConnectionString);

        return new SchedulerContext(optionsBuilder.Options);
    }
}