using System.ComponentModel.DataAnnotations;

namespace TrainSyncAPI.Dtos;

public class TemplateBaseDto
{
    [Required(ErrorMessage = "Title is required.")]
    [StringLength(
        100,
        MinimumLength = 2,
        ErrorMessage = "Title must be between 2 and 100 characters."
    )]
    public string Title { get; set; } = null!;

    [StringLength(1000, ErrorMessage = "Description can't exceed 1000 characters.")]
    public string? Description { get; set; }
}
