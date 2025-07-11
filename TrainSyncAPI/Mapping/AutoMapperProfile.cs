using AutoMapper;
using TrainSyncAPI.Dtos;
using TrainSyncAPI.Models;

namespace TrainSyncAPI.Mapping;

public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        // Exercise Model
        CreateMap<Exercise, ExerciseCreateDto>().ReverseMap();
        CreateMap<Exercise, ExerciseUpdateDto>().ReverseMap();

        // Template Model
        CreateMap<Template, TemplateCreateDto>().ReverseMap();
        CreateMap<Template, TemplateUpdateDto>().ReverseMap();

        // Workout Model
        CreateMap<Workout, WorkoutCreateDto>().ReverseMap();
        CreateMap<Workout, WorkoutUpdateDto>().ReverseMap();

        // WorkoutExercise Model
        CreateMap<WorkoutExercise, WorkoutExerciseCreateDto>().ReverseMap();
        CreateMap<WorkoutExercise, WorkoutExerciseUpdateDto>().ReverseMap();

        // TemplateExercise Model
        CreateMap<TemplateExercise, TemplateExerciseCreateDto>().ReverseMap();
        CreateMap<TemplateExercise, TemplateExerciseUpdateDto>().ReverseMap();

        // WorkoutExerciseSet Model
        CreateMap<WorkoutExerciseSet, WorkoutExerciseSetCreateDto>().ReverseMap();
        CreateMap<WorkoutExerciseSet, WorkoutExerciseSetUpdateDto>().ReverseMap();

        // TemplateExerciseSet Model
        CreateMap<TemplateExerciseSet, TemplateExerciseSetCreateDto>().ReverseMap();
        CreateMap<TemplateExerciseSet, TemplateExerciseSetUpdateDto>().ReverseMap();
    }
}