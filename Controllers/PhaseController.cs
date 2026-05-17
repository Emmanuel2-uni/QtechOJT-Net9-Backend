using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QtechOJT_Net9.Database;
using QtechOJT_Net9.DTO.Phase;
using QtechOJT_Net9.Models;

namespace QtechOJT_Net9.Controllers
{
    [Route("api/[controller]s")]
    [ApiController]
    public class PhaseController(KanbanDbContext context) : ControllerBase
    {
        private readonly KanbanDbContext _context = context;

        // GET /api/phases?grouping=dev
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] string? grouping)
        {
            var query = _context.Phases.AsQueryable();

            if (!string.IsNullOrWhiteSpace(grouping))
                query = query.Where(p => p.Grouping == grouping);

            var phases = await query
                .AsNoTracking()
                .OrderBy(p => p.SortOrder)
                .ToListAsync();

            return Ok(phases);
        }

        // POST /api/phases
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] PhaseDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Label))
                return BadRequest(new { message = "Label is required" });

            var phase = new Phase
            {
                Label = dto.Label.Trim(),
                SortOrder = dto.SortOrder,
                IsDefault = dto.IsDefault,
                IsFinal = dto.IsFinal,
                Grouping = dto.Grouping,
                DefaultStatusId = dto.DefaultStatusId
            };

            _context.Phases.Add(phase);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                return Conflict(new { message = "Phase label already exists" });
            }

            return CreatedAtAction(nameof(GetAll), new { id = phase.Id }, phase);
        }

        // PUT /api/phases/:id
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePhase(int id, [FromBody] UpdatePhaseDto dto)
        {
            var phase = await _context.Phases.FindAsync(id);
            if (phase is null)
                return NotFound(new { message = "Phase not found" });

            phase.Id = id;
            phase.Label = dto.Label;
            phase.SortOrder = dto.SortOrder;
            phase.IsDefault = dto.IsDefault;
            phase.IsFinal = dto.IsFinal;
            phase.Grouping = dto.Grouping;
            phase.DefaultStatusId = dto.DefaultStatusId;

            await _context.SaveChangesAsync();
            return Ok(phase);
        }

        // DELETE /api/phases/:id
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var isInUse = await _context.Main_Tasks.AnyAsync(t => t.PhaseId == id);
            if (isInUse)
                return BadRequest(new { message = "Cannot delete: This phase is currently in use by active tasks." });

            var phase = await _context.Phases.FindAsync(id);
            if (phase is null)
                return NotFound(new { message = "Phase not found" });

            _context.Phases.Remove(phase);
            await _context.SaveChangesAsync();
            return Ok(new { message = "Phase deleted" });
        }
    }
}
