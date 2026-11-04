
using Microsoft.EntityFrameworkCore;
using RaithuNestham.Models;

namespace RaithuNestham.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    // Database tables
    public DbSet<User> Users => Set<User>();

    public DbSet<DeviceRegistration> DeviceRegistrations
        => Set<DeviceRegistration>();

    public DbSet<Farmer> Farmers => Set<Farmer>();

    public DbSet<Field> Fields => Set<Field>();

    public DbSet<Crop> Crops => Set<Crop>();

    public DbSet<CropRecord> CropRecords => Set<CropRecord>();

    public DbSet<WeatherLog> WeatherLogs => Set<WeatherLog>();

    public DbSet<Fertilizer> Fertilizers => Set<Fertilizer>();

    public DbSet<Pesticide> Pesticides => Set<Pesticide>();

    public DbSet<GovernmentScheme> GovernmentSchemes
        => Set<GovernmentScheme>();

    public DbSet<EquipmentSubsidy> EquipmentSubsidies
        => Set<EquipmentSubsidy>();

    public DbSet<AIChat> AIChats => Set<AIChat>();

    public DbSet<MarketPrice> MarketPrices => Set<MarketPrice>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // User configuration
        modelBuilder.Entity<User>()
            .HasIndex(x => x.Username)
            .IsUnique();

        modelBuilder.Entity<User>()
            .HasOne(x => x.Farmer)
            .WithOne(x => x.User)
            .HasForeignKey<Farmer>(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // Device registration configuration
        modelBuilder.Entity<DeviceRegistration>()
            .Property(x => x.FcmToken)
            .HasMaxLength(2048)
            .IsRequired();

        modelBuilder.Entity<DeviceRegistration>()
            .Property(x => x.Platform)
            .HasMaxLength(20)
            .IsRequired();

        modelBuilder.Entity<DeviceRegistration>()
            .HasOne(x => x.User)
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<DeviceRegistration>()
            .HasIndex(x => x.UserId);

        // Farmer and field relationships
        modelBuilder.Entity<Farmer>()
            .HasMany(x => x.Fields)
            .WithOne(x => x.Farmer)
            .HasForeignKey(x => x.FarmerId)
            .OnDelete(DeleteBehavior.Cascade);

        // Field and crop record relationships
        modelBuilder.Entity<Field>()
            .HasMany(x => x.CropRecords)
            .WithOne(x => x.Field)
            .HasForeignKey(x => x.FieldId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Crop>()
            .HasMany(x => x.CropRecords)
            .WithOne(x => x.Crop)
            .HasForeignKey(x => x.CropId)
            .OnDelete(DeleteBehavior.Restrict);

        // Farmer and AI chat relationship
        modelBuilder.Entity<Farmer>()
            .HasMany<AIChat>()
            .WithOne(x => x.Farmer)
            .HasForeignKey(x => x.FarmerId)
            .OnDelete(DeleteBehavior.Cascade);

        // Field decimal precision
        modelBuilder.Entity<Field>()
            .Property(x => x.AreaInAcres)
            .HasPrecision(10, 2);

        // Crop record decimal precision
        modelBuilder.Entity<CropRecord>()
            .Property(x => x.ExpectedYield)
            .HasPrecision(18, 2);

        // Weather decimal precision
        modelBuilder.Entity<WeatherLog>()
            .Property(x => x.Temperature)
            .HasPrecision(10, 2);

        modelBuilder.Entity<WeatherLog>()
            .Property(x => x.Humidity)
            .HasPrecision(10, 2);

        modelBuilder.Entity<WeatherLog>()
            .Property(x => x.Rainfall)
            .HasPrecision(10, 2);

        // Market price decimal precision
        modelBuilder.Entity<MarketPrice>()
            .Property(x => x.MinPrice)
            .HasPrecision(18, 2);

        modelBuilder.Entity<MarketPrice>()
            .Property(x => x.MaxPrice)
            .HasPrecision(18, 2);

        modelBuilder.Entity<MarketPrice>()
            .Property(x => x.ModalPrice)
            .HasPrecision(18, 2);
    }
}
