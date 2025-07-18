using TrainSyncAPI.Enums;

namespace TrainSyncAPI.Dtos;

public class ExerciseWithTemplatesViewModel : ExerciseDto
{
    public List<TemplateListDto> Templates { get; set; } = new();
}
