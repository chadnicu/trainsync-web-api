using TrainSyncAPI.Enums;

namespace TrainSyncAPI.Models.Dtos;

public class ExerciseCreateDto
{
    public string Title { get; set; } = null!;
    public string? Instructions { get; set; }
    public string? Url { get; set; }
    public WeightUnit WeightUnit { get; set; } = WeightUnit.Kilograms;
    public IntensityUnit IntensityUnit { get; set; } = IntensityUnit.RepsInReserve;
}