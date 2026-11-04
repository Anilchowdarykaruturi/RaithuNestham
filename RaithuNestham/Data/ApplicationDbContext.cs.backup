using Microsoft.EntityFrameworkCore;
using RaithuNestham.Models;

namespace RaithuNestham.Data;
using RaithuNestham.Models;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();

    public DbSet<Farmer> Farmers => Set<Farmer>();

    public DbSet<Field> Fields => Set<Field>();

    public DbSet<Crop> Crops => Set<Crop>();

    public DbSet<CropRecord> CropRecords => Set<CropRecord>();

    public DbSet<WeatherLog> WeatherLogs => Set<WeatherLog>();

    public DbSet<Fertilizer> Fertilizers => Set<Fertilizer>();

    public DbSet<Pesticide> Pesticides => Set<Pesticide>();

    public DbSet<GovernmentScheme> GovernmentSchemes
        => Set<GovernmentScheme>(); public DbSet<EquipmentSubsidy> EquipmentSubsidies { get; set; }

    public DbSet<AIChat> AIChats => Set<AIChat>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>()
            .HasIndex(x => x.Username)
            .IsUnique();

        modelBuilder.Entity<User>()
            .HasOne(x => x.Farmer)
            .WithOne(x => x.User)
            .HasForeignKey<Farmer>(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Farmer>()
            .HasMany(x => x.Fields)
            .WithOne(x => x.Farmer)
            .HasForeignKey(x => x.FarmerId)
            .OnDelete(DeleteBehavior.Cascade);

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

        modelBuilder.Entity<Farmer>()
            .HasMany<AIChat>()
            .WithOne(x => x.Farmer)
            .HasForeignKey(x => x.FarmerId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Field>()
    .Property(x => x.AreaInAcres)
    .HasPrecision(10, 2);

        modelBuilder.Entity<CropRecord>()
            .Property(x => x.ExpectedYield)
            .HasPrecision(18, 2);

        // -----------------------------------------
        // Weather decimal precision
        // -----------------------------------------

        modelBuilder.Entity<WeatherLog>()
            .Property(x => x.Temperature)
            .HasPrecision(10, 2);

        modelBuilder.Entity<WeatherLog>()
            .Property(x => x.Humidity)
            .HasPrecision(10, 2);

        modelBuilder.Entity<WeatherLog>()
            .Property(x => x.Rainfall)
            .HasPrecision(10, 2);
    }
}