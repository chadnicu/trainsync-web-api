using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TrainSyncAPI.Data;
using TrainSyncAPI.Dtos;
using TrainSyncAPI.Extensions;
using TrainSyncAPI.Models;

namespace TrainSyncAPI.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class ExercisesController : ControllerBase
{
    private readonly TrainSyncContext _context;
    private readonly IMapper _mapper;

    public ExercisesController(TrainSyncContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    // GET: api/Exercises
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ExerciseWithTemplatesDto>>> GetExercises()
    {
        var userId = User.GetClerkUserId();

        return await _context.Exercises
            .Where(e => e.UserId == userId || e.IsPublic)
            .Select(e => new ExerciseWithTemplatesDto
            {
                Id = e.Id,
                Title = e.Title,
                Instructions = e.Instructions,
                Url = e.Url,
                WeightUnit = e.WeightUnit,
                IntensityUnit = e.IntensityUnit,
                IsPublic = e.IsPublic,
                Templates = e.TemplateExercises
                    .Where(te => te.UserId == userId)
                    .Select(te =>
                        new TemplateListDto
                        {
                            Id = te.Id,
                            Title = te.Template.Title
                        }
                    )
                    .ToList()
            })
            .ToListAsync();
    }

    // GET: api/Exercises/5
    [HttpGet("{exerciseId}")]
    public async Task<ActionResult<ExerciseWithWorkoutSetsDto>> GetExercise([FromRoute] long exerciseId)
    {
        var userId = User.GetClerkUserId();

        var exercise = await _context.Exercises
            .Where(e => e.Id == exerciseId && (e.UserId == userId || e.IsPublic))
            .Select(e => new ExerciseWithWorkoutSetsDto
            {
                Id = e.Id,
                Title = e.Title,
                Instructions = e.Instructions,
                Url = e.Url,
                WeightUnit = e.WeightUnit,
                IntensityUnit = e.IntensityUnit,
                IsPublic = e.IsPublic,
                Sets = e.WorkoutExercises.Select(we =>
                    new WorkoutExerciseWithSetsDto
                    {
                        Id = we.Id,
                        Order = we.Order,
                        Instructions = we.Instructions,
                        Comment = we.Comment,
                        Workout = new WorkoutDto
                        {
                            Id = we.WorkoutId,
                            Title = we.Workout.Title,
                            Description = we.Workout.Description,
                            ProgrammedDate = we.Workout.ProgrammedDate,
                            StartTime = we.Workout.StartTime,
                            EndTime = we.Workout.EndTime,
                            Comment = we.Workout.Comment
                        },
                        // I probably don't need this but don't feel like creating a separate dto
                        Exercise = new ExerciseDto
                        {
                            Id = we.ExerciseId,
                            Title = we.Exercise.Title,
                            Instructions = we.Exercise.Instructions,
                            IsPublic = we.Exercise.IsPublic,
                            Url = we.Exercise.Url,
                            WeightUnit = we.Exercise.WeightUnit,
                            IntensityUnit = we.Exercise.IntensityUnit
                        },
                        Sets = we.Sets.Select(s =>
                            new WorkoutExerciseSetDto
                            {
                                Id = s.Id,
                                Reps = s.Reps,
                                Weight = s.Weight,
                                Intensity = s.Intensity,
                                Comment = s.Comment,
                                WeightUnit = s.WeightUnit,
                                IntensityUnit = s.IntensityUnit
                            }
                        ).ToList()
                    }
                ).ToList()
            })
            .FirstOrDefaultAsync();

        if (exercise == null) return NotFound();

        return exercise;
    }

    // PUT: api/Exercises/5
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPut("{exerciseId}")]
    public async Task<IActionResult> PutExercise([FromRoute] long exerciseId,
        [FromBody] ExerciseUpdateDto dto)
    {
        var userId = User.GetClerkUserId();

        var existingExercise = await _context.Exercises
            .FirstOrDefaultAsync(e => e.Id == exerciseId && e.UserId == userId);

        if (existingExercise == null) return NotFound();

        _mapper.Map(dto, existingExercise);

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!ExerciseExists(exerciseId, userId)) return NotFound();

            throw;
        }

        return NoContent();
    }

    // POST: api/Exercises
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPost]
    public async Task<ActionResult<Exercise>> PostExercise(
        [FromBody] ExerciseCreateDto dto)
    {
        var userId = User.GetClerkUserId();

        var exercise = _mapper.Map<Exercise>(dto);
        exercise.UserId = userId;

        _context.Exercises.Add(exercise);
        await _context.SaveChangesAsync();

        var resultDto = _mapper.Map<ExerciseDto>(exercise);
        return CreatedAtAction(nameof(GetExercise), new { exerciseId = exercise.Id }, resultDto);
    }

    // DELETE: api/Exercises/5
    [HttpDelete("{exerciseId}")]
    public async Task<IActionResult> DeleteExercise([FromRoute] long exerciseId)
    {
        var userId = User.GetClerkUserId();
        var exercise = await _context.Exercises.FirstOrDefaultAsync(e => e.Id == exerciseId && e.UserId == userId);

        if (exercise == null) return NotFound();

        _context.Exercises.Remove(exercise);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool ExerciseExists(long exerciseId, string userId)
    {
        return _context.Exercises.Any(e => e.Id == exerciseId && e.UserId == userId);
    }
}