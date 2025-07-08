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
public class WorkoutsController : ControllerBase
{
    private readonly TrainSyncContext _context;

    public WorkoutsController(TrainSyncContext context)
    {
        _context = context;
    }

    // GET: api/Workouts
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Workout>>> GetWorkouts()
    {
        var userId = User.GetClerkUserId();

        return await _context.Workouts.Where(w => w.UserId == userId).ToListAsync();
    }

    // GET: api/Workouts/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Workout>> GetWorkout([FromRoute] long id)
    {
        var userId = User.GetClerkUserId();
        var workout = await _context.Workouts.FirstOrDefaultAsync(w => w.Id == id && w.UserId == userId);

        if (workout == null) return NotFound();

        return workout;
    }

    // PUT: api/Workouts/5
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPut("{id}")]
    public async Task<IActionResult> PutWorkout([FromRoute] long id, WorkoutUpdateDto dto)
    {
        var userId = User.GetClerkUserId();

        var existingWorkout = await _context.Workouts
            .FirstOrDefaultAsync(w => w.Id == id && w.UserId == userId);

        if (existingWorkout == null) return NotFound();

        existingWorkout.Title = dto.Title;
        existingWorkout.Description = dto.Description;
        existingWorkout.ProgrammedDate = dto.ProgrammedDate;
        existingWorkout.StartTime = dto.StartDate;
        existingWorkout.EndTime = dto.EndDate;
        existingWorkout.Comment = dto.Comment;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!WorkoutExists(id, userId)) return NotFound();

            throw;
        }

        return NoContent();
    }

    // POST: api/Workouts
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPost]
    public async Task<ActionResult<Workout>> PostWorkout(WorkoutCreateDto dto)
    {
        var userId = User.GetClerkUserId();

        var workout = new Workout
        {
            Title = dto.Title,
            Description = dto.Description,
            ProgrammedDate = dto.ProgrammedDate,
            StartTime = dto.StartDate,
            EndTime = dto.EndDate,
            Comment = dto.Comment,
            UserId = userId
        };

        _context.Workouts.Add(workout);
        await _context.SaveChangesAsync();

        return CreatedAtAction("GetWorkout", new { id = workout.Id }, workout);
    }

    // DELETE: api/Workouts/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteWorkout([FromRoute] long id)
    {
        var userId = User.GetClerkUserId();
        var workout = await _context.Workouts.FirstOrDefaultAsync(w => w.Id == id && w.UserId == userId);

        if (workout == null) return NotFound();

        _context.Workouts.Remove(workout);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool WorkoutExists(long id, string userId)
    {
        return _context.Workouts.Any(w => w.Id == id && w.UserId == userId);
    }
}