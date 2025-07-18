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
public class TemplatesController : ControllerBase
{
    private readonly TrainSyncContext _context;

    public TemplatesController(TrainSyncContext context)
    {
        _context = context;
    }

    // GET: api/Templates
    [HttpGet]
    public async Task<ActionResult<IEnumerable<TemplateWithExercisesViewModel>>> GetTemplates()
    {
        var userId = User.GetClerkUserId();

        var templatesWithExerciseListAndSetCount = await _context.Templates
            .Where(t => t.UserId == userId)
            .Select(t => new TemplateWithExercisesViewModel
            {
                Id = t.Id,
                Title = t.Title,
                Description = t.Description,
                IsPublic = t.IsPublic,
                Exercises = t.Exercises.Select(te =>
                    new TemplateExerciseWithSetCountDto
                    {
                        Id = te.Id,
                        Order = te.Order,
                        Title = te.Exercise.Title,
                        SetsCount = te.Sets.Count
                    }).ToList()
            })
            .OrderByDescending(t => t.Id)
            .ToListAsync();

        return templatesWithExerciseListAndSetCount;
    }

    // GET: api/Templates/5
    [HttpGet("{templateId}")]
    public async Task<ActionResult<TemplateWithExercisesAndSetsViewModel>> GetTemplate([FromRoute] long templateId)
    {
        var userId = User.GetClerkUserId();

        var template = await _context.Templates
            .Where(t => t.Id == templateId && (t.UserId == userId || t.IsPublic))
            .Select(t => new TemplateWithExercisesAndSetsViewModel
            {
                Id = t.Id,
                Title = t.Title,
                Description = t.Description,
                IsPublic = t.IsPublic,
                Exercises = t.Exercises.Select(te => new TemplateExerciseWithSetsViewModel
                    {
                        Id = te.Id,
                        Order = te.Order,
                        Instructions = te.Instructions,
                        Exercise = new ExerciseDto
                        {
                            Id = te.ExerciseId,
                            Title = te.Exercise.Title,
                            Instructions = te.Exercise.Instructions,
                            IsPublic = te.Exercise.IsPublic,
                            Url = te.Exercise.Url,
                            WeightUnit = te.Exercise.WeightUnit,
                            IntensityUnit = te.Exercise.IntensityUnit
                        },
                        Sets = te.Sets.Select(s => new TemplateExerciseSetDto
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

        if (template == null) return NotFound();

        return template;
    }

    // PUT: api/Templates/5
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPut("{templateId}")]
    public async Task<IActionResult> PutTemplate([FromRoute] long templateId, TemplateUpdateDto dto)
    {
        var userId = User.GetClerkUserId();

        var existingTemplate = await _context.Templates
            .FirstOrDefaultAsync(t => t.Id == templateId && t.UserId == userId);

        if (existingTemplate == null) return NotFound();

        existingTemplate.Title = dto.Title;
        existingTemplate.Description = dto.Description;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!TemplateExists(templateId, userId)) return NotFound();

            throw;
        }

        return NoContent();
    }

    // POST: api/Templates
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPost]
    public async Task<ActionResult<TemplateWithExercisesViewModel>> PostTemplate(TemplateCreateDto dto)
    {
        var userId = User.GetClerkUserId();

        var template = new Template
        {
            Title = dto.Title,
            Description = dto.Description,
            IsPublic = false,
            UserId = userId
        };

        _context.Templates.Add(template);
        await _context.SaveChangesAsync();

        var result = new TemplateWithExercisesViewModel
        {
            Id = template.Id,
            Title = template.Title,
            Description = template.Description,
            IsPublic = template.IsPublic,
            Exercises = []
        };

        return result;
    }

    // DELETE: api/Templates/5
    [HttpDelete("{templateId}")]
    public async Task<IActionResult> DeleteTemplate([FromRoute] long templateId)
    {
        var userId = User.GetClerkUserId();
        var template = await _context.Templates.FirstOrDefaultAsync(t => t.Id == templateId && t.UserId == userId);

        if (template == null) return NotFound();

        _context.Templates.Remove(template);
        await _context.SaveChangesAsync();

        return NoContent();
    }


    // POST: api/Templates/5/Exercises
    [HttpPost("{templateId}/Exercises")]
    public async Task<ActionResult<TemplateExerciseWithSetsViewModel>> PostTemplateExercise([FromRoute] long templateId,
        [FromBody] TemplateExerciseCreateDto dto)
    {
        var userId = User.GetClerkUserId();

        var templateTask = _context.Templates
            .FirstOrDefaultAsync(t => t.Id == templateId && t.UserId == userId);
        var exerciseTask = _context.Exercises
            .FirstOrDefaultAsync(e => e.Id == dto.ExerciseId && (e.UserId == userId || e.IsPublic));

        // await Task.WhenAll(templateTask, exerciseTask);

        var template = await templateTask;
        var exercise = await exerciseTask;

        if (template == null || exercise == null) return NotFound();

        var templateExercise = new TemplateExercise
        {
            Order = dto.Order,
            ExerciseId = dto.ExerciseId,
            Instructions = dto.Instructions,
            TemplateId = templateId,
            Sets = dto.Sets.Select(s => new TemplateExerciseSet
            {
                UserId = userId,
                IntensityUnit = s.IntensityUnit,
                WeightUnit = s.WeightUnit,
                Intensity = s.Intensity,
                Reps = s.Reps,
                Weight = s.Weight
            }).ToList(),
            UserId = userId
        };

        _context.TemplateExercises.Add(templateExercise);
        await _context.SaveChangesAsync();

        var result = new TemplateExerciseWithSetsViewModel
        {
            Id = templateExercise.Id,
            Order = templateExercise.Order,
            Instructions = templateExercise.Instructions,
            Exercise = new ExerciseDto
            {
                Id = templateExercise.ExerciseId,
                IsPublic = templateExercise.Exercise.IsPublic,
                Instructions = templateExercise.Exercise.Instructions,
                IntensityUnit = templateExercise.Exercise.IntensityUnit,
                WeightUnit = templateExercise.Exercise.WeightUnit,
                Title = templateExercise.Exercise.Title,
                Url = templateExercise.Exercise.Url
            },
            Sets = templateExercise.Sets.Select(s =>
                new TemplateExerciseSetDto
                {
                    Id = s.Id,
                    IntensityUnit = s.IntensityUnit, WeightUnit = s.WeightUnit, Intensity = s.Intensity, Reps = s.Reps,
                    Weight = s.Weight
                }
            ).ToList()
        };

        return result;
    }

    // PUT: api/Templates/Exercises/5
    [HttpPut("Exercises/{templateExerciseId}")]
    public async Task<IActionResult> PutTemplateExercise([FromRoute] long templateExerciseId,
        TemplateExerciseUpdateDto dto)
    {
        var userId = User.GetClerkUserId();

        var existingTemplateExercise = await _context.TemplateExercises
            .FirstOrDefaultAsync(t => t.Id == templateExerciseId && t.UserId == userId);

        if (existingTemplateExercise == null) return NotFound();

        existingTemplateExercise.Instructions = dto.Instructions;
        existingTemplateExercise.Order = dto.Order;
        existingTemplateExercise.ExerciseId = dto.ExerciseId;
        existingTemplateExercise.Sets = dto.Sets.Select(s => new TemplateExerciseSet
        {
            UserId = userId,
            IntensityUnit = s.IntensityUnit,
            WeightUnit = s.WeightUnit,
            Intensity = s.Intensity,
            Reps = s.Reps,
            Weight = s.Weight
        }).ToList();

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!TemplateExerciseExists(templateExerciseId, userId)) return NotFound();

            throw;
        }

        return NoContent();
    }

    // DELETE: api/Templates/Exercises/5
    [HttpDelete("Exercises/{templateExerciseId}")]
    public async Task<IActionResult> DeleteTemplateExercise([FromRoute] long templateExerciseId)
    {
        var userId = User.GetClerkUserId();
        var templateExercise =
            await _context.TemplateExercises.FirstOrDefaultAsync(t => t.Id == templateExerciseId && t.UserId == userId);

        if (templateExercise == null) return NotFound();

        _context.TemplateExercises.Remove(templateExercise);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    // PUT: api/Templates/5/Exercises/Reorder
    [HttpPut("{templateId}/Exercises/Reorder")]
    public async Task<IActionResult> ReorderTemplateExercises(
        [FromRoute] long templateId,
        [FromBody] List<TemplateExerciseReorderDto> reorderList)
    {
        var userId = User.GetClerkUserId();

        var idsToUpdate = reorderList.Select(x => x.TemplateExerciseId).ToList();

        var exercises = await _context.TemplateExercises
            .Where(te => te.TemplateId == templateId && te.UserId == userId && idsToUpdate.Contains(te.Id))
            .ToListAsync();

        if (exercises.Count != reorderList.Count)
            return NotFound("One or more exercises not found or not owned by user.");

        foreach (var item in reorderList)
        {
            var exercise = exercises.First(te => te.Id == item.TemplateExerciseId);
            exercise.Order = item.Order;
        }

        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool TemplateExists(long templateId, string userId)
    {
        return _context.Templates.Any(e => e.Id == templateId && e.UserId == userId);
    }

    private bool TemplateExerciseExists(long templateExerciseId, string userId)
    {
        return _context.TemplateExercises.Any(e => e.Id == templateExerciseId && e.UserId == userId);
    }
}