using System.ComponentModel.DataAnnotations;

namespace TrainSyncAPI.Dtos;

public class TemplateExerciseUpdateDto
{
    [Required]
    [Range(0, double.MaxValue, ErrorMessage = "Order must be a non-negative number.")]
    public double Order { get; set; }

    [Required(ErrorMessage = "ExerciseId is required.")]
    public long ExerciseId { get; set; }

    public string? Instructions { get; set; }

    public List<TemplateExerciseSetCreateDto>? Sets { get; set; }
}