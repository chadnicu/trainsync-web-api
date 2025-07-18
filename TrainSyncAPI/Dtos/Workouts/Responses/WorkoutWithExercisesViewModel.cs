namespace TrainSyncAPI.Dtos;

public class WorkoutWithExercisesViewModel : WorkoutBaseViewModel
{
    public List<WorkoutExerciseWithSetCountDto> Exercises { get; set; } = new();
}
