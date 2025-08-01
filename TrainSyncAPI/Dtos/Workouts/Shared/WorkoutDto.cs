namespace TrainSyncAPI.Dtos;

public class WorkoutDto
{
    public long Id { get; set; }
    public string Title { get; set; } = null!;
    public string? Description { get; set; }
    public DateOnly ProgrammedDate { get; set; }
    public TimeOnly? StartTime { get; set; }
    public TimeOnly? EndTime { get; set; }
    public string? Comment { get; set; }
}