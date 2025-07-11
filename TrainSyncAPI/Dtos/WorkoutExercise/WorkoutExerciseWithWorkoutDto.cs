namespace TrainSyncAPI.Dtos;

public class WorkoutExerciseWithWorkoutDto
{
    public long Id { get; set; }
    public double Order { get; set; }
    public string? Instructions { get; set; }
    public string? Comment { get; set; }

    public WorkoutDto? Workout { get; set; }
}