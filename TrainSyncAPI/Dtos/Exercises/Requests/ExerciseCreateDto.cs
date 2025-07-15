using System.ComponentModel.DataAnnotations;
using TrainSyncAPI.Enums;

namespace TrainSyncAPI.Dtos;

public class ExerciseCreateDto
{
    [Required(ErrorMessage = "Title is required.")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Title must be between 2 and 100 characters.")]
    public string Title { get; set; } = null!;

    [StringLength(1000, ErrorMessage = "Instructions can't exceed 1000 characters.")]
    public string? Instructions { get; set; }

    [Url(ErrorMessage = "Please provide a valid URL.")]
    [StringLength(2048, ErrorMessage = "URL is too long.")]
    public string? Url { get; set; }

    [Required(ErrorMessage = "Weight unit is required.")]
    [EnumDataType(typeof(WeightUnit), ErrorMessage = "Invalid weight unit.")]
    public WeightUnit WeightUnit { get; set; } = WeightUnit.Kilograms;

    [Required(ErrorMessage = "Intensity unit is required.")]
    [EnumDataType(typeof(IntensityUnit), ErrorMessage = "Invalid intensity unit.")]
    public IntensityUnit IntensityUnit { get; set; } = IntensityUnit.RepetitionsInReserve;
}