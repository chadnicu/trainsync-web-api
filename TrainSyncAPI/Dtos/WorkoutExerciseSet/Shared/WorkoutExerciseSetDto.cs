using TrainSyncAPI.Enums;

namespace TrainSyncAPI.Dtos;

public class WorkoutExerciseSetDto
{
    public long Id { get; set; }

    public double? Reps { get; set; }

    public double? Weight { get; set; }

    public double? Intensity { get; set; }

    public string? Comment { get; set; }

    public required WeightUnit WeightUnit { get; set; } = WeightUnit.Kilograms;

    public required IntensityUnit IntensityUnit { get; set; } = IntensityUnit.RepetitionsInReserve;
}