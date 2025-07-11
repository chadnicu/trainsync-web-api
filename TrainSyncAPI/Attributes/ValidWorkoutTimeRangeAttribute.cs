using System.ComponentModel.DataAnnotations;
using TrainSyncAPI.Dtos;

namespace TrainSyncAPI.Attributes;

public class ValidWorkoutTimeRangeAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        var workout = (WorkoutCreateDto)validationContext.ObjectInstance;

        if (workout.StartTime.HasValue && workout.EndTime.HasValue)
            if (workout.EndTime <= workout.StartTime)
                return new ValidationResult("End time must be later than start time.");

        return ValidationResult.Success;
    }
}