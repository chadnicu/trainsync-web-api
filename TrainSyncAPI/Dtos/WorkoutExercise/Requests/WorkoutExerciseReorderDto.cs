using System.ComponentModel.DataAnnotations;

namespace TrainSyncAPI.Dtos;

public class WorkoutExerciseReorderDto
{
    [Required] public long WorkoutExerciseId { get; set; }

    [Required]
    [Range(0, double.MaxValue, ErrorMessage = "Order must be a non-negative number.")]
    public double Order { get; set; }
}