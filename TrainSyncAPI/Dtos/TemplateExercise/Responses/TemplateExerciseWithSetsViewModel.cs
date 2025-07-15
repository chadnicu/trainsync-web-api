namespace TrainSyncAPI.Dtos;

public class TemplateExerciseWithSetsViewModel
{
    public long Id { get; set; }

    public double Order { get; set; }

    public string? Instructions { get; set; }

    public ExerciseDto? Exercise { get; set; }

    public List<TemplateExerciseSetDto> Sets { get; set; } = new();
}