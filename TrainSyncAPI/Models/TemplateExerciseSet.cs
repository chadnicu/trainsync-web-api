using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TrainSyncAPI.Enums;

namespace TrainSyncAPI.Models;

[Table("template_exercise_set")]
public class TemplateExerciseSet
{
    [Key]
    [Column("id")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; set; }

    [Column("reps")] public double? Reps { get; set; }

    [Column("weight")] public double? Weight { get; set; }

    [Column("intensity")] public double? Intensity { get; set; }

    [Required] [Column("weight_unit")] public required WeightUnit WeightUnit { get; set; } = WeightUnit.Kilograms;

    [Required]
    [Column("intensity_unit")]
    public required IntensityUnit IntensityUnit { get; set; } = IntensityUnit.RepetitionsInReserve;

    [Required] [Column("user_id")] public required string UserId { get; set; }

    [Required]
    [Column("template_exercise_id")]
    public long TemplateExerciseId { get; set; }

    [ForeignKey(nameof(TemplateExerciseId))]
    public TemplateExercise? TemplateExercise { get; set; }
}