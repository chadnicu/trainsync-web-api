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
public class WorkoutsController : ControllerBase
{
    private readonly TrainSyncContext _context;

    public WorkoutsController(TrainSyncContext context)
    {
        _context = context;
    }

    // GET: api/Workouts
    [HttpGet]
    public async Task<ActionResult<IEnumerable<WorkoutWithExercisesViewModel>>> GetWorkouts()
    {
        var userId = User.GetClerkUserId();

        var workoutWithExerciseListAndSetCount =
            await _context.Workouts
                .Where(w => w.UserId == userId)
                .Select(w => new WorkoutWithExercisesViewModel
                {
                    Id = w.Id,
                    Title = w.Title,
                    Description = w.Description,
                    ProgrammedDate = w.ProgrammedDate,
                    StartTime = w.StartTime,
                    EndTime = w.EndTime,
                    Comment = w.Comment,
                    Exercises = w.Exercises.Select(e => new WorkoutExerciseWithSetCountDto
                        {
                            Id = e.Id,
                            Order = e.Order,
                            Title = e.Exercise.Title,
                            SetsCount = e.Sets.Count
                        }
                    ).ToList()
                })
                .OrderByDescending(w => w.Id)
                .ToListAsync();

        return workoutWithExerciseListAndSetCount;
    }

    // GET: api/Workouts/5
    [HttpGet("{workoutId}")]
    public async Task<ActionResult<WorkoutWithExercisesAndSetsViewModel>> GetWorkout([FromRoute] long workoutId)
    {
        var userId = User.GetClerkUserId();

        var workout = await _context.Workouts
            .Where(w => w.Id == workoutId && w.UserId == userId)
            .Select(w => new WorkoutWithExercisesAndSetsViewModel
            {
                Id = w.Id,
                Title = w.Title,
                Description = w.Description,
                ProgrammedDate = w.ProgrammedDate,
                StartTime = w.StartTime,
                EndTime = w.EndTime,
                Comment = w.Comment,
                Exercises = w.Exercises.Select(we => new WorkoutExerciseWithExerciseAndSetsViewModel
                    {
                        Id = we.Id,
                        Order = we.Order,
                        Instructions = we.Instructions,
                        Comment = we.Comment,
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
                        Sets = we.Sets.Select(s => new WorkoutExerciseSetDto
                            {
                                Id = s.Id,
                                WeightUnit = s.WeightUnit,
                                IntensityUnit = s.IntensityUnit,
                                Reps = s.Reps,
                                Weight = s.Weight,
                                Intensity = s.Intensity
                            }
                        ).ToList()
                    }
                ).ToList()
            })
            .FirstOrDefaultAsync();

        if (workout == null) return NotFound();

        return workout;
    }

    // PUT: api/Workouts/5
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPut("{workoutId}")]
    public async Task<IActionResult> PutWorkout([FromRoute] long workoutId, WorkoutUpdateDto dto)
    {
        var userId = User.GetClerkUserId();

        var existingWorkout = await _context.Workouts
            .FirstOrDefaultAsync(w => w.Id == workoutId && w.UserId == userId);

        if (existingWorkout == null) return NotFound();

        existingWorkout.Title = dto.Title;
        existingWorkout.Description = dto.Description;
        existingWorkout.ProgrammedDate = dto.ProgrammedDate;
        existingWorkout.StartTime = dto.StartTime;
        existingWorkout.EndTime = dto.EndTime;
        existingWorkout.Comment = dto.Comment;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!WorkoutExists(workoutId, userId)) return NotFound();

            throw;
        }

        return NoContent();
    }

    // POST: api/Workouts
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPost]
    public async Task<ActionResult<WorkoutWithExercisesViewModel>> PostWorkout(WorkoutCreateDto dto)
    {
        var userId = User.GetClerkUserId();

        var workout = new Workout
        {
            Title = dto.Title,
            Description = dto.Description,
            ProgrammedDate = dto.ProgrammedDate,
            StartTime = dto.StartTime,
            EndTime = dto.EndTime,
            Comment = dto.Comment, UserId = userId
        };

        _context.Workouts.Add(workout);
        await _context.SaveChangesAsync();

        var result = new WorkoutWithExercisesViewModel
        {
            Id = workout.Id,
            Title = workout.Title,
            Description = workout.Description,
            ProgrammedDate = workout.ProgrammedDate,
            StartTime = workout.StartTime,
            EndTime = workout.EndTime,
            Comment = workout.Comment,
            Exercises = []
        };

        return result;
    }

    // DELETE: api/Workouts/5
    [HttpDelete("{workoutId}")]
    public async Task<IActionResult> DeleteWorkout([FromRoute] long workoutId)
    {
        var userId = User.GetClerkUserId();
        var workout = await _context.Workouts.FirstOrDefaultAsync(w => w.Id == workoutId && w.UserId == userId);

        if (workout == null) return NotFound();

        _context.Workouts.Remove(workout);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    // POST: api/Workouts/5/Exercises
    [HttpPost("{workoutId}/Exercises")]
    public async Task<ActionResult<WorkoutExerciseWithExerciseAndSetsViewModel>> PostWorkoutExercise(
        [FromRoute] long workoutId,
        [FromBody] WorkoutExerciseCreateDto dto
    )
    {
        var userId = User.GetClerkUserId();

        var workoutTask = _context.Workouts
            .FirstOrDefaultAsync(w => w.Id == workoutId && w.UserId == userId);
        var exerciseTask = _context.Exercises
            .FirstOrDefaultAsync(e => e.Id == dto.ExerciseId && (e.UserId == userId || e.IsPublic));

        // await Task.WhenAll(workoutTask, exerciseTask);

        var workout = await workoutTask;
        var exercise = await exerciseTask;

        if (workout == null || exercise == null) return NotFound();

        // Oare sa cer si sets cand se adauga un exercitiu in workout?
        var workoutExercise = new WorkoutExercise
        {
            Order = dto.Order,
            ExerciseId = dto.ExerciseId,
            Instructions = dto.Instructions,
            Comment = dto.Comment,
            UserId = userId,
            WorkoutId = workoutId,
            Sets = []
        };

        _context.WorkoutExercises.Add(workoutExercise);
        await _context.SaveChangesAsync();

        var result = new WorkoutExerciseWithExerciseAndSetsViewModel
        {
            Id = workoutExercise.Id,
            Order = workoutExercise.Order,
            Instructions = workoutExercise.Instructions,
            Comment = workoutExercise.Comment,
            Exercise = new ExerciseDto
            {
                Id = workoutExercise.ExerciseId,
                Title = workoutExercise.Exercise.Title,
                Instructions = workoutExercise.Exercise.Instructions,
                Url = workoutExercise.Exercise.Url,
                WeightUnit = workoutExercise.Exercise.WeightUnit,
                IntensityUnit = workoutExercise.Exercise.IntensityUnit,
                IsPublic = workoutExercise.Exercise.IsPublic
            },
            Sets = workoutExercise.Sets.Select(s => new
                WorkoutExerciseSetDto
                {
                    Id = s.Id,
                    Reps = s.Reps,
                    WeightUnit = s.WeightUnit,
                    IntensityUnit = s.IntensityUnit,
                    Comment = s.Comment,
                    Weight = s.Weight,
                    Intensity = s.Intensity
                }
            ).ToList()
        };

        return result;
    }

    // PUT: api/Workouts/Exercises/5
    [HttpPut("Exercises/{workoutExerciseId}")]
    public async Task<IActionResult> PutWorkoutExercise(
        [FromRoute] long workoutExerciseId,
        WorkoutExerciseUpdateDto dto)
    {
        var userId = User.GetClerkUserId();

        var existingWorkoutExercise = await _context.WorkoutExercises
            .FirstOrDefaultAsync(t => t.Id == workoutExerciseId && t.UserId == userId);

        if (existingWorkoutExercise == null) return NotFound();

        existingWorkoutExercise.Order = dto.Order;
        existingWorkoutExercise.ExerciseId = dto.ExerciseId;
        existingWorkoutExercise.Instructions = dto.Instructions;
        existingWorkoutExercise.Comment = dto.Comment;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!WorkoutExerciseExists(workoutExerciseId, userId)) return NotFound();

            throw;
        }

        return NoContent();
    }

    // DELETE: api/Workouts/Exercises/5
    [HttpDelete("Exercises/{workoutExerciseId}")]
    public async Task<IActionResult> DeleteWorkoutExercise([FromRoute] long workoutExerciseId)
    {
        var userId = User.GetClerkUserId();
        var workoutExercise =
            await _context.WorkoutExercises.FirstOrDefaultAsync(t => t.Id == workoutExerciseId && t.UserId == userId);

        if (workoutExercise == null) return NotFound();

        _context.WorkoutExercises.Remove(workoutExercise);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    // PUT: api/Workouts/5/Exercises/Reorder
    [HttpPut("{workoutId}/Exercises/Reorder")]
    public async Task<IActionResult> ReorderWorkoutExercises(
        [FromRoute] long workoutId,
        [FromBody] List<WorkoutExerciseReorderDto> reorderList)
    {
        var userId = User.GetClerkUserId();

        var idsToUpdate = reorderList.Select(x => x.WorkoutExerciseId).ToList();

        var exercises = await _context.WorkoutExercises
            .Where(we => we.WorkoutId == workoutId && we.UserId == userId && idsToUpdate.Contains(we.Id))
            .ToListAsync();

        if (exercises.Count != reorderList.Count)
            return NotFound("One or more exercises not found or not owned by user.");

        foreach (var item in reorderList)
        {
            var exercise = exercises.First(te => te.Id == item.WorkoutExerciseId);
            exercise.Order = item.Order;
        }

        await _context.SaveChangesAsync();

        return NoContent();
    }

    // POST: api/Workouts/FromTemplate
    [HttpPost("FromTemplate")]
    public async Task<ActionResult<WorkoutWithExercisesViewModel>> CreateWorkoutFromTemplate(
        [FromBody] WorkoutFromTemplateCreateDto dto)
    {
        var userId = User.GetClerkUserId();

        var template = await _context.Templates
            .Include(t => t.Exercises)
            .ThenInclude(te => te.Sets)
            .FirstOrDefaultAsync(t => t.Id == dto.TemplateId && (t.UserId == userId || t.IsPublic));
        if (template == null) return NotFound();

        var workout = new Workout
        {
            Title = template.Title,
            Description = template.Description,
            UserId = userId,
            ProgrammedDate = dto.ProgrammedDate,
            Exercises = template.Exercises.Select(te => new WorkoutExercise
                {
                    ExerciseId = te.ExerciseId,
                    Order = te.Order,
                    Instructions = te.Instructions,
                    UserId = userId,
                    Sets = te.Sets.Select(s => new WorkoutExerciseSet
                        {
                            Reps = s.Reps,
                            WeightUnit = s.WeightUnit,
                            Weight = s.Weight,
                            IntensityUnit = s.IntensityUnit,
                            Intensity = s.Intensity,
                            UserId = userId
                        }
                    ).ToList()
                }
            ).ToList()
        };
        _context.Workouts.Add(workout);

        await _context.SaveChangesAsync();

        var result = new WorkoutWithExercisesViewModel
        {
            Id = workout.Id,
            Title = workout.Title,
            Description = workout.Description,
            Comment = workout.Comment,
            ProgrammedDate = workout.ProgrammedDate,
            StartTime = workout.StartTime,
            EndTime = workout.EndTime,
            Exercises = workout.Exercises.Select(e => new WorkoutExerciseWithSetCountDto
            {
                Id = e.Id,
                Order = e.Order,
                Title = e.Exercise.Title,
                SetsCount = e.Sets.Count
            }).ToList()
        };

        return result;
    }

    private bool WorkoutExists(long workoutId, string userId)
    {
        return _context.Workouts.Any(w => w.Id == workoutId && w.UserId == userId);
    }

    private bool WorkoutExerciseExists(long workoutExerciseId, string userId)
    {
        return _context.WorkoutExercises.Any(e => e.Id == workoutExerciseId && e.UserId == userId);
    }
}