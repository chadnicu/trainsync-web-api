using System.ComponentModel.DataAnnotations;
using TrainSyncAPI.Attributes;

namespace TrainSyncAPI.Dtos;

[ValidWorkoutTimeRange]
public class WorkoutCreateDto : WorkoutBaseDto { }
