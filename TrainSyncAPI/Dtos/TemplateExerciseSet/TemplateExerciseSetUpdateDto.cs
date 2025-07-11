using System.ComponentModel.DataAnnotations;
using TrainSyncAPI.Enums;

namespace TrainSyncAPI.Dtos;

public class TemplateExerciseSetUpdateDto
{
    [Range(0.01, double.MaxValue, ErrorMessage = "Reps must be greater than 0.")]
    public double? Reps { get; set; }

    [Range(0.0, double.MaxValue, ErrorMessage = "Weight cannot be negative.")]
    public double? Weight { get; set; }

    [Range(0.0, double.MaxValue, ErrorMessage = "Intensity cannot be negative.")]
    public double? Intensity { get; set; }

    [Required(ErrorMessage = "Weight unit is required.")]
    [EnumDataType(typeof(WeightUnit), ErrorMessage = "Invalid weight unit.")]
    public WeightUnit WeightUnit { get; set; } = WeightUnit.Kilograms;

    [Required(ErrorMessage = "Intensity unit is required.")]
    [EnumDataType(typeof(IntensityUnit), ErrorMessage = "Invalid intensity unit.")]
    public IntensityUnit IntensityUnit { get; set; } = IntensityUnit.RepetitionsInReserve;
}