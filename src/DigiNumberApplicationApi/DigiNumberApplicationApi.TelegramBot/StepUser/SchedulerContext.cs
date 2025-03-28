using Microsoft.EntityFrameworkCore;

namespace DigiNumberApplicationApi.TelegramBot.StepUser;

public class SchedulerContext : DbContext
{
    public SchedulerContext(DbContextOptions<SchedulerContext> options) : base(options) { }

    public DbSet<UserStep> UserStep { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var userSessionBuilder = modelBuilder.Entity<UserStep>();
        userSessionBuilder.ToTable("UserStep");
        userSessionBuilder.HasKey(x => x.Id);
        userSessionBuilder.HasIndex(x => x.ChatId).IsUnique();
        userSessionBuilder.Property(x => x.ChatId).IsRequired();
        userSessionBuilder.Property(x => x.Step).IsRequired().HasMaxLength(100);
        userSessionBuilder.Property(x => x.ExpierTime).IsRequired();
    }
}