namespace TrainSyncAPI.Dtos;

public class TemplateExerciseWithSetCountDto
{
    public long Id { get; set; }
    public double Order { get; set; }
    public string Title { get; set; } = null!;
    public int SetsCount { get; set; }
}