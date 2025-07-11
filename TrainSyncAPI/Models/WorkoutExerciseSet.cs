using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TrainSyncAPI.Enums;

namespace TrainSyncAPI.Models;

[Table("workout_exercise_set")]
public class WorkoutExerciseSet
{
    [Key]
    [Column("id")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; set; }

    [Column("reps")] public double? Reps { get; set; }

    [Column("weight")] public double? Weight { get; set; }

    [Column("intensity")] public double? Intensity { get; set; }

    [Column("comment")] public string? Comment { get; set; }

    [Required] [Column("weight_unit")] public required WeightUnit WeightUnit { get; set; } = WeightUnit.Kilograms;

    [Required]
    [Column("intensity_unit")]
    public required IntensityUnit IntensityUnit { get; set; } = IntensityUnit.RepetitionsInReserve;

    [Required] [Column("user_id")] public required string UserId { get; set; }

    [Required]
    [Column("workout_exercise_id")]
    public long WorkoutExerciseId { get; set; }

    [ForeignKey(nameof(WorkoutExerciseId))]
    public WorkoutExercise? WorkoutExercise { get; set; }
}