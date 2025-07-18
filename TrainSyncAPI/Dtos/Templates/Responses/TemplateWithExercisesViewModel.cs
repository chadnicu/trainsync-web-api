namespace TrainSyncAPI.Dtos;

public class TemplateWithExercisesViewModel : TemplateBaseViewModel
{
    public List<TemplateExerciseWithSetCountDto> Exercises { get; set; } = new();
}
