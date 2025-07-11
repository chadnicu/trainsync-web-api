using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TrainSyncAPI.Models;

[Table("template_exercise")]
// Actually, user might want to do circuits
// [Index(nameof(ExerciseId), nameof(TemplateId), IsUnique = true)]
public class TemplateExercise
{
    [Key]
    [Column("id")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; set; }

    [Required] [Column("order")] public required double Order { get; set; }

    [Column("instructions")] public string? Instructions { get; set; }

    [Required] [Column("user_id")] public required string UserId { get; set; }
    [Required] [Column("template_id")] public required long TemplateId { get; set; }
    [ForeignKey(nameof(TemplateId))] public Template? Template { get; set; }

    [Required] [Column("exercise_id")] public required long ExerciseId { get; set; }
    [ForeignKey(nameof(ExerciseId))] public Exercise? Exercise { get; set; }

    public List<TemplateExerciseSet> Sets { get; set; } = new();
}