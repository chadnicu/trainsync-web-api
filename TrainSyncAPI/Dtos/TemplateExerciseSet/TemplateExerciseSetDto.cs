using TrainSyncAPI.Enums;

namespace TrainSyncAPI.Dtos;

public class TemplateExerciseSetDto
{
    public long Id { get; set; }

    public double? Reps { get; set; }

    public double? Weight { get; set; }

    public double? Intensity { get; set; }

    public WeightUnit WeightUnit { get; set; } = WeightUnit.Kilograms;

    public IntensityUnit IntensityUnit { get; set; } = IntensityUnit.RepetitionsInReserve;
}