namespace TrainSyncAPI.Dtos;

public class TemplateWithExercisesDto
{
    public long Id { get; set; }

    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public bool IsPublic { get; set; } = false;

    public List<TemplateExerciseWithSetCountDto> Exercises { get; set; } = new();
}