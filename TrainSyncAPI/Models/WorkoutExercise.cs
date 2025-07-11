using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TrainSyncAPI.Models;

[Table("workout_exercise")]
// Actually, user might want to do circuits
// [Index(nameof(ExerciseId), nameof(WorkoutId), IsUnique = true)]
public class WorkoutExercise
{
    [Key]
    [Column("id")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; set; }

    [Required] [Column("order")] public required double Order { get; set; }

    [Column("instructions")] public string? Instructions { get; set; }

    [Column("comment")] public string? Comment { get; set; }

    [Required] [Column("user_id")] public required string UserId { get; set; }

    [Required] [Column("workout_id")] public long WorkoutId { get; set; }
    [ForeignKey(nameof(WorkoutId))] public Workout? Workout { get; set; }

    [Required] [Column("exercise_id")] public required long ExerciseId { get; set; }
    [ForeignKey(nameof(ExerciseId))] public Exercise? Exercise { get; set; }

    public List<WorkoutExerciseSet> Sets { get; set; } = new();
}