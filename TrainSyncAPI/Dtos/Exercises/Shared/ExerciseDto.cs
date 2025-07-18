using TrainSyncAPI.Enums;

namespace TrainSyncAPI.Dtos;

public class ExerciseDto : ExerciseBaseDto
{
    public long Id { get; set; }
    public bool IsPublic { get; set; }
}
