namespace TrainSyncAPI.Dtos;

public class WorkoutWithExercisesAndSetsViewModel : WorkoutBaseViewModel
{
    public List<WorkoutExerciseWithExerciseAndSetsViewModel> Exercises { get; set; } = new();
}
