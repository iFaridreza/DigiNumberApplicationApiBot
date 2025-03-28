using DigiNumberApplicationApi.TelegramBot.Core.Domian;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace DigiNumberApplicationApi.TelegramBot.Infrastructer;
public class Context : DbContext
{
    public Context(DbContextOptions<Context> options) : base(options)
    {
    }

    public DbSet<Sudo> Sudo { get; set; }
    public DbSet<User> User { get; set; }
    public DbSet<VirtualNumber> VirtualNumber { get; set; }
    public DbSet<OrderVirtualNumber> OrderVirtualNumber { get; set; }
    public DbSet<VirtualNumberDetails> VirtualNumberDetails { get; set; }
    public DbSet<VirtualSessionDetails> VirtualSessionDetails { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var sudoBuilder = modelBuilder.Entity<Sudo>();
        sudoBuilder.ToTable("Sudo");
        sudoBuilder.HasKey(x => x.Id);
        sudoBuilder.HasIndex(x => x.ChatId).IsUnique();
        sudoBuilder.Property(x => x.ChatId).IsRequired().HasMaxLength(20);

        var userBuilder = modelBuilder.Entity<User>();
        userBuilder.ToTable("User");
        userBuilder.HasKey(x => x.Id);
        userBuilder.HasIndex(x => x.ChatId).IsUnique();
        userBuilder.Property(x => x.ChatId).IsRequired().HasMaxLength(20);
        userBuilder.Property(x => x.PhoneNumber).HasMaxLength(20);
        userBuilder.Property(x => x.IsVerify).IsRequired();
        userBuilder.HasMany<OrderVirtualNumber>().WithOne(x => x.User).HasForeignKey(x => x.UserId);

        var virtualNumberBuilder = modelBuilder.Entity<VirtualNumber>();
        virtualNumberBuilder.ToTable("VirtualNumber");
        virtualNumberBuilder.HasKey(x => x.Id);
        virtualNumberBuilder.HasIndex(x => x.Number).IsUnique();
        virtualNumberBuilder.Property(x => x.CountryCode).IsRequired().HasMaxLength(6);
        virtualNumberBuilder.Property(x => x.Number).IsRequired().HasMaxLength(20);
        virtualNumberBuilder.Property(x => x.EStatusOrder).HasConversion(new EnumToStringConverter<EStatusOrder>()).IsRequired();
        virtualNumberBuilder.HasOne(x => x.VirtualSessionDetails).WithMany().HasForeignKey(x => x.VirtualSessionDetailsId);

        var orderVirtualNumberBuilder = modelBuilder.Entity<OrderVirtualNumber>();
        orderVirtualNumberBuilder.ToTable("OrderVirtualNumber");
        orderVirtualNumberBuilder.HasKey(x => x.Id);
        orderVirtualNumberBuilder.Property(x => x.Date).IsRequired().HasMaxLength(15);
        orderVirtualNumberBuilder.Property(x => x.Price).IsRequired();
        orderVirtualNumberBuilder.HasOne(x => x.User).WithMany().HasForeignKey(x => x.UserId);
        orderVirtualNumberBuilder.HasOne(x => x.VirtualNumber).WithOne().HasForeignKey<OrderVirtualNumber>(x => x.VirtualNumberId);


        var virtualNumberDetailsBuilder = modelBuilder.Entity<VirtualNumberDetails>();
        virtualNumberDetailsBuilder.ToTable("VirtualNumberDetails");
        virtualNumberDetailsBuilder.HasKey(x => x.Id);
        virtualNumberDetailsBuilder.Property(x => x.CountryCode).IsRequired().HasMaxLength(6);
        virtualNumberDetailsBuilder.Property(x => x.Flag).IsRequired().HasMaxLength(60);
        virtualNumberDetailsBuilder.Property(x => x.Price).IsRequired();

        var virtualSessionDetailsBuilder = modelBuilder.Entity<VirtualSessionDetails>();
        virtualSessionDetailsBuilder.ToTable("VirtualSessionDetails");
        virtualSessionDetailsBuilder.HasKey(x => x.Id);
        virtualSessionDetailsBuilder.Property(x => x.ApiId).IsRequired().HasMaxLength(50);
        virtualSessionDetailsBuilder.Property(x => x.ApiHash).IsRequired().HasMaxLength(50);
        virtualSessionDetailsBuilder.Property(x => x.Password2Fa).IsRequired().HasMaxLength(100);
    }
}
