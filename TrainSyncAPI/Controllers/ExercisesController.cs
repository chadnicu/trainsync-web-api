using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TrainSyncAPI.Data;
using TrainSyncAPI.Extensions;
using TrainSyncAPI.Models;
using TrainSyncAPI.Models.Dtos;

namespace TrainSyncAPI.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class ExercisesController : ControllerBase
{
    private readonly TrainSyncContext _context;

    public ExercisesController(TrainSyncContext context)
    {
        _context = context;
    }

    // GET: api/Exercises
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Exercise>>> GetExercises()
    {
        var userId = User.GetClerkUserId();

        return await _context.Exercises.Where(e => e.UserId == userId).ToListAsync();
    }

    // GET: api/Exercises/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Exercise>> GetExercise([FromRoute] long id)
    {
        var userId = User.GetClerkUserId();
        var exercise = await _context.Exercises.FirstOrDefaultAsync(e => e.Id == id && e.UserId == userId);

        if (exercise == null) return NotFound();

        return exercise;
    }

    // PUT: api/Exercises/5
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPut("{id}")]
    public async Task<IActionResult> PutExercise([FromRoute] long id,
        [FromBody] ExerciseUpdateDto dto)
    {
        var userId = User.GetClerkUserId();

        var existingExercise = await _context.Exercises
            .FirstOrDefaultAsync(e => e.Id == id && e.UserId == userId);

        if (existingExercise == null) return NotFound();

        existingExercise.Title = dto.Title;
        existingExercise.Instructions = dto.Instructions;
        existingExercise.Url = dto.Url;
        existingExercise.WeightUnit = dto.WeightUnit;
        existingExercise.IntensityUnit = dto.IntensityUnit;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!ExerciseExists(id, userId)) return NotFound();

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

        var exercise = new Exercise
        {
            Title = dto.Title,
            Instructions = dto.Instructions,
            Url = dto.Url,
            WeightUnit = dto.WeightUnit,
            IntensityUnit = dto.IntensityUnit,
            UserId = userId
        };

        _context.Exercises.Add(exercise);
        await _context.SaveChangesAsync();

        return CreatedAtAction("GetExercise", new { id = exercise.Id }, exercise);
    }

    // DELETE: api/Exercises/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteExercise([FromRoute] long id)
    {
        var userId = User.GetClerkUserId();
        var exercise = await _context.Exercises.FirstOrDefaultAsync(e => e.Id == id && e.UserId == userId);

        if (exercise == null) return NotFound();

        _context.Exercises.Remove(exercise);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool ExerciseExists(long id, string userId)
    {
        return _context.Exercises.Any(e => e.Id == id && e.UserId == userId);
    }
}