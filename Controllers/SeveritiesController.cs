using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QtechOJT_Net9.Database;
using QtechOJT_Net9.Models;
using QtechOJT_Net9.DTO.Severity;

namespace QtechOJT_Net9.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SeveritiesController(KanbanDbContext context) : ControllerBase
    {
        private readonly KanbanDbContext _context = context;

        // -- GET /api/severities ------------------------
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var severities = await _context.Severities
                .AsNoTracking()
                .OrderBy(s => s.SortOrder)
                .ToListAsync();

            return Ok(severities);
        }

        // -- GET /api/severities/{id} ------------------------
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var severity = await _context.Severities
                .FindAsync(id); // For mutables, we have no AsNoTracking here because FindAsync is not compatible (due to operation/mutability)
            if (severity is null) return NotFound(new { message = "Severity not found" });

            return Ok(severity);
        }

        // -- POST /api/severities ------------------------
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] SeverityRequestDto req)
        {
            if (string.IsNullOrWhiteSpace(req.Label))
                return BadRequest(new { message = "Label is required" });

            bool isDuplicate = await _context.Severities.AnyAsync(s => s.Label == req.Label.Trim());
            if (isDuplicate) return Conflict(new { message = "A severity with that label already exists" });

            var severity = new Severity
            {
                Label = req.Label.Trim(),
                Color = req.Color,
                SortOrder = req.SortOrder
            };

            _context.Severities.Add(severity);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = severity.Id }, severity);
        }

        // -- PUT /api/severities/{id} ------------------------
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] SeverityRequestDto req)
        {
            if (string.IsNullOrWhiteSpace(req.Label))
                return BadRequest(new { message = "Label is required" });

            var severity = await _context.Severities.FindAsync(id);
            if (severity is null) return NotFound(new { message = "Severity not found" });

            bool isDuplicate = await _context.Severities.AnyAsync(s => s.Label == req.Label.Trim() && s.Id != id);
            if (isDuplicate) return Conflict(new { message = "A severity with that label already exists" });

            severity.Label = req.Label.Trim();
            severity.Color = req.Color;
            severity.SortOrder = req.SortOrder;

            await _context.SaveChangesAsync();

            return Ok(severity);
        }

        // -- DELETE /api/severities/{id} ------------------------
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            bool isInUse = await _context.Main_Tasks.AnyAsync(t => t.SeverityId == id);
            if (isInUse) return Conflict(new { message = "Cannot delete: this severity is used by one or more tasks." });

            var severity = await _context.Severities.FindAsync(id);
            if (severity is null) return NotFound(new { message = "Severity not found" });

            _context.Severities.Remove(severity);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Severity deleted" });
        }
    }
}
