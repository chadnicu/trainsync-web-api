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
public class TemplatesController : ControllerBase
{
    private readonly TrainSyncContext _context;
    private readonly IMapper _mapper;

    public TemplatesController(TrainSyncContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    // GET: api/Templates
    [HttpGet]
    public async Task<ActionResult<IEnumerable<TemplateWithExercisesDto>>> GetTemplates()
    {
        var userId = User.GetClerkUserId();

        return await _context.Templates
            .Where(t => t.UserId == userId)
            .Select(t => new TemplateWithExercisesDto
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
            .ToListAsync();
    }

    // GET: api/Templates/5
    [HttpGet("{templateId}")]
    public async Task<ActionResult<TemplateWithExercisesAndSetsDto>> GetTemplate([FromRoute] long templateId)
    {
        var userId = User.GetClerkUserId();

        var template = await _context.Templates
            .Where(t => t.Id == templateId && (t.UserId == userId || t.IsPublic))
            .Select(t => new TemplateWithExercisesAndSetsDto
            {
                Id = t.Id,
                Title = t.Title,
                Description = t.Description,
                IsPublic = t.IsPublic,
                Exercises = t.Exercises.Select(te => new TemplateExerciseWithSetsDto
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

        _mapper.Map(dto, existingTemplate);

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
    public async Task<ActionResult<Template>> PostTemplate(TemplateCreateDto dto)
    {
        var userId = User.GetClerkUserId();

        var template = _mapper.Map<Template>(dto);
        template.UserId = userId;

        _context.Templates.Add(template);
        await _context.SaveChangesAsync();

        var resultDto = _mapper.Map<TemplateDto>(template);
        return CreatedAtAction(nameof(GetTemplate), new { templateId = template.Id }, resultDto);
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
    public async Task<IActionResult> PostTemplateExercise([FromRoute] long templateId,
        [FromBody] TemplateExerciseCreateDto dto)
    {
        var userId = User.GetClerkUserId();

        var templateTask = _context.Templates
            .FirstOrDefaultAsync(t => t.Id == templateId && t.UserId == userId);
        var exerciseTask = _context.Exercises
            .FirstOrDefaultAsync(e => e.Id == dto.ExerciseId && (e.UserId == userId || e.IsPublic));

        await Task.WhenAll(templateTask, exerciseTask);

        var template = await templateTask;
        var exercise = await exerciseTask;

        if (template == null || exercise == null) return NotFound();

        var templateExercise = _mapper.Map<TemplateExercise>(dto);
        templateExercise.TemplateId = templateId;
        templateExercise.UserId = userId;

        if (templateExercise.Sets.Count > 0)
            foreach (var set in templateExercise.Sets)
                set.UserId = userId;

        _context.TemplateExercises.Add(templateExercise);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetTemplate), new { templateId }, templateExercise);
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

        _mapper.Map(dto, existingTemplateExercise);

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