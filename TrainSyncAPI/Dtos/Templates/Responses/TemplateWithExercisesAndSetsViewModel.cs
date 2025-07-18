namespace TrainSyncAPI.Dtos;

public class TemplateWithExercisesAndSetsViewModel : TemplateBaseViewModel
{
    public List<TemplateExerciseWithSetsViewModel> Exercises { get; set; } = new();
}
