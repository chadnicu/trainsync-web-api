namespace TrainSyncAPI.Dtos;

public class TemplateWithExercisesAndSetsViewModel
{
    public long Id { get; set; }

    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public bool IsPublic { get; set; } = false;

    public List<TemplateExerciseWithSetsViewModel> Exercises { get; set; } = new();
}