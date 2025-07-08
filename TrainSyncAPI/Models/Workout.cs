using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TrainSyncAPI.Models;

[Table("workout")]
public class Workout
{
    [Key]
    [Column("id")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; set; }

    [Required] [Column("title")] public required string Title { get; set; }

    [Column("description")] public string? Description { get; set; }

    [Column("programmed_date", TypeName = "date")]
    public DateOnly ProgrammedDate { get; set; }

    [Column("start_time", TypeName = "time")]
    public TimeOnly? StartTime { get; set; }

    [Column("end_time", TypeName = "time")]
    public TimeOnly? EndTime { get; set; }

    [Column("comment")] public string? Comment { get; set; }

    [Column("user_id")] public required string UserId { get; set; }
}