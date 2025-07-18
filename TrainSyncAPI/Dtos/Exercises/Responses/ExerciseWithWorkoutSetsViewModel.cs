using TrainSyncAPI.Enums;

namespace TrainSyncAPI.Dtos;

public class ExerciseWithWorkoutSetsViewModel : ExerciseDto
{
    public List<WorkoutExerciseWithWorkoutAndSetsViewModel> Sets { get; set; } = [];
}
