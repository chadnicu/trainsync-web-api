using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using TrainSyncAPI.Enums;

namespace TrainSyncAPI.Models;

[Table("exercise")]
[Index(nameof(Title), nameof(UserId), IsUnique = true)]
public class Exercise
{
    [Key]
    [Column("id")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; set; }

    [Required] [Column("title")] public required string Title { get; set; }

    [Column("instructions")] public string? Instructions { get; set; }

    [Column("url")] public string? Url { get; set; }

    [Required] [Column("weight_unit")] public required WeightUnit WeightUnit { get; set; } = WeightUnit.Kilograms;

    [Required]
    [Column("intensity_unit")]
    public required IntensityUnit IntensityUnit { get; set; } = IntensityUnit.RepetitionsInReserve;

    [Column("is_public")] public bool IsPublic { get; set; } = false;

    [Required] [Column("user_id")] public required string UserId { get; set; }

    public List<WorkoutExercise> WorkoutExercises { get; set; } = new();

    public List<TemplateExercise> TemplateExercises { get; set; } = new();
}