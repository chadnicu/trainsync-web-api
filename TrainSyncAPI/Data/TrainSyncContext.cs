using Microsoft.EntityFrameworkCore;
using TrainSyncAPI.Enums;
using TrainSyncAPI.Models;

namespace TrainSyncAPI.Data;

public class TrainSyncContext : DbContext
{
    public TrainSyncContext(DbContextOptions<TrainSyncContext> options) : base(options)
    {
    }

    public DbSet<Exercise> Exercises { get; set; }
    public DbSet<Workout> Workouts { get; set; }
    public DbSet<Template> Templates { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Exercise>(entity =>
        {
            entity.Property(e => e.WeightUnit)
                .HasDefaultValue(WeightUnit.Kilograms)
                .HasConversion<string>();

            entity.Property(e => e.IntensityUnit)
                .HasDefaultValue(IntensityUnit.RepsInReserve)
                .HasConversion<string>();
        });
    }
}