namespace TrainSyncAPI.Dtos;

public class WorkoutWithExercisesAndSetsDto
{
    public long Id { get; set; }

    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public DateOnly ProgrammedDate { get; set; }

    public TimeOnly? StartTime { get; set; }

    public TimeOnly? EndTime { get; set; }

    public string? Comment { get; set; }

    public List<WorkoutExerciseWithSetsDto> Exercises { get; set; } = new();
}