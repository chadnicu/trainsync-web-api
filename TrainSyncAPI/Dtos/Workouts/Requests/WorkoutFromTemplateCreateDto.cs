using System.ComponentModel.DataAnnotations;

namespace TrainSyncAPI.Dtos;

public class WorkoutFromTemplateCreateDto
{
    [Required] public long TemplateId { get; set; }

    [Required] public DateOnly ProgrammedDate { get; set; }
}