namespace TrainSyncAPI.Models.Dtos;

public class WorkoutUpdateDto
{
    public string Title { get; set; } = null!;
    public string? Description { get; set; }
    public DateOnly ProgrammedDate { get; set; }
    public TimeOnly? StartDate { get; set; }
    public TimeOnly? EndDate { get; set; }
    public string? Comment { get; set; }
}