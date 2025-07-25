using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TrainSyncAPI.Models;

[Table("template")]
public class Template
{
    [Key]
    [Column("id")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; set; }

    [Required] [Column("title")] public required string Title { get; set; }

    [Column("description")] public string? Description { get; set; }

    [Column("is_public")] public bool IsPublic { get; set; } = false;

    [Required] [Column("user_id")] public required string UserId { get; set; }

    public List<TemplateExercise> Exercises { get; set; } = new();
}