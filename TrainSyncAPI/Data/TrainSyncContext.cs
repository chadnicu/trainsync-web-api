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
    public DbSet<WorkoutExercise> WorkoutExercises { get; set; }
    public DbSet<TemplateExercise> TemplateExercises { get; set; }
    public DbSet<WorkoutExerciseSet> WorkoutExercisesSet { get; set; }
    public DbSet<TemplateExerciseSet> TemplateExercisesSet { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Store enums as strings in DB for Exercise table
        modelBuilder.Entity<Exercise>(entity =>
        {
            entity.Property(e => e.WeightUnit)
                .HasDefaultValue(WeightUnit.Kilograms)
                .HasConversion<string>();

            entity.Property(e => e.IntensityUnit)
                .HasDefaultValue(IntensityUnit.RepetitionsInReserve)
                .HasConversion<string>();
        });

        // Store enums as strings in DB for WorkoutExerciseSet table
        modelBuilder.Entity<WorkoutExerciseSet>(entity =>
        {
            entity.Property(e => e.WeightUnit)
                .HasDefaultValue(WeightUnit.Kilograms)
                .HasConversion<string>();

            entity.Property(e => e.IntensityUnit)
                .HasDefaultValue(IntensityUnit.RepetitionsInReserve)
                .HasConversion<string>();
        });

        // Store enums as strings in DB for TemplateExerciseSet table
        modelBuilder.Entity<TemplateExerciseSet>(entity =>
        {
            entity.Property(e => e.WeightUnit)
                .HasDefaultValue(WeightUnit.Kilograms)
                .HasConversion<string>();

            entity.Property(e => e.IntensityUnit)
                .HasDefaultValue(IntensityUnit.RepetitionsInReserve)
                .HasConversion<string>();
        });

        // Set ON DELETE CASCADE for Workout > WorkoutExercise
        modelBuilder.Entity<WorkoutExercise>()
            .HasOne(we => we.Workout)
            .WithMany(w => w.Exercises)
            .HasForeignKey(we => we.WorkoutId)
            .OnDelete(DeleteBehavior.Cascade);

        // Set ON DELETE CASCADE for Exercise > WorkoutExercise
        modelBuilder.Entity<WorkoutExercise>()
            .HasOne(we => we.Exercise)
            .WithMany(e => e.WorkoutExercises)
            .HasForeignKey(we => we.ExerciseId)
            .OnDelete(DeleteBehavior.Cascade);

        // Set ON DELETE CASCADE for Template > TemplateExercise
        modelBuilder.Entity<TemplateExercise>()
            .HasOne(we => we.Template)
            .WithMany(w => w.Exercises)
            .HasForeignKey(we => we.TemplateId)
            .OnDelete(DeleteBehavior.Cascade);

        // Set ON DELETE CASCADE for Exercise > WorkoutExercise
        modelBuilder.Entity<TemplateExercise>()
            .HasOne(we => we.Exercise)
            .WithMany(e => e.TemplateExercises)
            .HasForeignKey(we => we.ExerciseId)
            .OnDelete(DeleteBehavior.Cascade);

        // Set ON DELETE CASCADE for WorkoutExercise > WorkoutExerciseSet
        modelBuilder.Entity<WorkoutExerciseSet>()
            .HasOne(wes => wes.WorkoutExercise)
            .WithMany(we => we.Sets)
            .HasForeignKey(wes => wes.WorkoutExerciseId)
            .OnDelete(DeleteBehavior.Cascade);

        // Set ON DELETE CASCADE for TemplateExercise > TemplateExerciseSet
        modelBuilder.Entity<TemplateExerciseSet>()
            .HasOne(wes => wes.TemplateExercise)
            .WithMany(we => we.Sets)
            .HasForeignKey(wes => wes.TemplateExerciseId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}