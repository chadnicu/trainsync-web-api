using TrainSyncAPI.Enums;

namespace TrainSyncAPI.Dtos;

public class ExerciseDto
{
    public long Id { get; set; }

    public string Title { get; set; } = null!;

    public string? Instructions { get; set; }

    public string? Url { get; set; }

    public WeightUnit WeightUnit { get; set; } = WeightUnit.Kilograms;

    public IntensityUnit IntensityUnit { get; set; } = IntensityUnit.RepetitionsInReserve;

    public bool IsPublic { get; set; }
}