using System.ComponentModel.DataAnnotations;
using TrainSyncAPI.Attributes;

namespace TrainSyncAPI.Dtos;

public class WorkoutBaseDto
{
    [Required]
    [StringLength(
        100,
        MinimumLength = 1,
        ErrorMessage = "Title must be between 1 and 100 characters."
    )]
    public string Title { get; set; } = null!;

    [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters.")]
    public string? Description { get; set; }

    [Required(ErrorMessage = "ProgrammedDate is required.")]
    public DateOnly ProgrammedDate { get; set; }

    [DataType(DataType.Time)]
    public TimeOnly? StartTime { get; set; }

    [DataType(DataType.Time)]
    public TimeOnly? EndTime { get; set; }

    [StringLength(500, ErrorMessage = "Comment cannot exceed 500 characters.")]
    public string? Comment { get; set; }
}
