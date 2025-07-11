namespace TrainSyncAPI.Dtos;

public class WorkoutExerciseWithSetsDto
{
    public long Id { get; set; }
    public double Order { get; set; }
    public string? Instructions { get; set; }
    public string? Comment { get; set; }

    public ExerciseDto? Exercise { get; set; }

    public WorkoutDto? Workout { get; set; }

    public List<WorkoutExerciseSetDto> Sets { get; set; } = new();
}