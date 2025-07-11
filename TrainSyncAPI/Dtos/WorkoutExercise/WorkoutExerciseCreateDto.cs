using System.ComponentModel.DataAnnotations;

namespace TrainSyncAPI.Dtos;

public class WorkoutExerciseCreateDto
{
    [Range(0, double.MaxValue, ErrorMessage = "Order must be 0 or greater.")]
    public double Order { get; set; }

    [Required(ErrorMessage = "WorkoutId is required.")]
    [Range(1, long.MaxValue, ErrorMessage = "WorkoutId must be a positive number.")]
    public long WorkoutId { get; set; }

    [Required(ErrorMessage = "ExerciseId is required.")]
    [Range(1, long.MaxValue, ErrorMessage = "ExerciseId must be a positive number.")]
    public long ExerciseId { get; set; }

    [MaxLength(1000, ErrorMessage = "Instructions cannot be longer than 1000 characters.")]
    public string? Instructions { get; set; }

    [MaxLength(500, ErrorMessage = "Comment cannot be longer than 500 characters.")]
    public string? Comment { get; set; }
}