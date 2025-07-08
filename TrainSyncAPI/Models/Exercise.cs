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

    [Column("weight_unit")] public WeightUnit WeightUnit { get; set; } = WeightUnit.Kilograms;

    [Column("intensity_unit")] public IntensityUnit IntensityUnit { get; set; } = IntensityUnit.RepsInReserve;

    [Required] [Column("user_id")] public required string UserId { get; set; }
}