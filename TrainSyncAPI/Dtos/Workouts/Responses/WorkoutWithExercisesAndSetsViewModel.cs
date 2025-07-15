namespace TrainSyncAPI.Dtos;

public class WorkoutWithExercisesAndSetsViewModel
{
    public long Id { get; set; }

    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public DateOnly ProgrammedDate { get; set; }

    public TimeOnly? StartTime { get; set; }

    public TimeOnly? EndTime { get; set; }

    public string? Comment { get; set; }

    public List<WorkoutExerciseWithExerciseAndSetsViewModel> Exercises { get; set; } = new();
}