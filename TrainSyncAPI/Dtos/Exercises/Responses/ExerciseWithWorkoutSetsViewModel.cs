using TrainSyncAPI.Enums;

namespace TrainSyncAPI.Dtos;

public class ExerciseWithWorkoutSetsViewModel
{
    public long Id { get; set; }
    public string Title { get; set; } = null!;
    public string? Instructions { get; set; }
    public string? Url { get; set; }
    public WeightUnit WeightUnit { get; set; }
    public IntensityUnit IntensityUnit { get; set; }
    public bool IsPublic { get; set; }

    public List<WorkoutExerciseWithWorkoutAndSetsViewModel> Sets { get; set; } = [];
}