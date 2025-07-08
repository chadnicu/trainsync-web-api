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
public class TemplatesController : ControllerBase
{
    private readonly TrainSyncContext _context;

    public TemplatesController(TrainSyncContext context)
    {
        _context = context;
    }

    // GET: api/Templates
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Template>>> GetTemplate()
    {
        var userId = User.GetClerkUserId();

        return await _context.Templates.Where(t => t.UserId == userId).ToListAsync();
    }

    // GET: api/Templates/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Template>> GetTemplate([FromRoute] long id)
    {
        var userId = User.GetClerkUserId();
        var template = await _context.Templates.FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);

        if (template == null) return NotFound();

        return template;
    }

    // PUT: api/Templates/5
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPut("{id}")]
    public async Task<IActionResult> PutTemplate([FromRoute] long id, TemplateUpdateDto dto)
    {
        var userId = User.GetClerkUserId();

        var existingTemplate = await _context.Templates
            .FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);

        if (existingTemplate == null) return NotFound();

        existingTemplate.Title = dto.Title;
        existingTemplate.Description = dto.Description;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!TemplateExists(id, userId)) return NotFound();

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

        var template = new Template
        {
            Title = dto.Title,
            Description = dto.Description,
            UserId = userId
        };

        _context.Templates.Add(template);
        await _context.SaveChangesAsync();

        return CreatedAtAction("GetTemplate", new { id = template.Id }, template);
    }

    // DELETE: api/Templates/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTemplate([FromRoute] long id)
    {
        var userId = User.GetClerkUserId();
        var template = await _context.Templates.FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);

        if (template == null) return NotFound();

        _context.Templates.Remove(template);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool TemplateExists(long id, string userId)
    {
        return _context.Templates.Any(e => e.Id == id && e.UserId == userId);
    }
}