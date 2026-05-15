using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QtechOJT_Net9.Database;
using QtechOJT_Net9.Models;
using QtechOJT_Net9.DTO.Status;

namespace QtechOJT_Net9.Controllers
{
    [Route("api/[controller]es")]
    [ApiController]
    public class StatusController(KanbanDbContext context) : ControllerBase
    {
        private readonly KanbanDbContext _context = context;

        [HttpGet]
        public async Task<IActionResult> GetStatuses()
        {
            var statuses = await _context.Statuses
                 .AsNoTracking()
                .OrderBy(s => s.SortOrder)
                .ToListAsync();

            return Ok(statuses);
        }

        // --POST /api/statuses ------------------------
        [HttpPost]
        public async Task<IActionResult> CreateStatus([FromBody] StatusRequestDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Label))
                return BadRequest(new { message = "Label is required" });

            bool labelExists = await _context.Statuses
                .AnyAsync(s => s.Label == dto.Label.Trim());

            if (labelExists)
                return Conflict(new { message = "Status label already exists" });

            var status = new Status
            {
                Label = dto.Label.Trim(),
                Color = dto.Color ?? "#6b7280",
                SortOrder = dto.SortOrder,
                IsDefault = dto.IsDefault,
                IsFinal = dto.IsFinal,
            };

            _context.Statuses.Add(status);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetStatuses), new { id = status.Id }, status);
        }

        // -- PUT /api/statuses/:id (FROM THE FRONTEND) ------------------------
        // Updates a status
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] StatusRequestDto dto)
        {
            var status = await _context.Statuses.FindAsync(id);
            if (status is null)
                return NotFound(new { message = "Status not found" });

            status.Label = dto.Label;
            status.Color = dto.Color ?? "#6b7280";
            status.SortOrder = dto.SortOrder;
            status.IsDefault = dto.IsDefault;
            status.IsFinal = dto.IsFinal;

            await _context.SaveChangesAsync();
            return Ok(status);
        }

        // -- DELETE /api/statuses/:id (FROM THE FRONTEND) ------------------------
        // Blocked if any tasks are using this status.
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteStatus(int id)
        {
            var status = await _context.Statuses.FindAsync(id);
            if (status is null)
                return NotFound(new { message = "Status not found" });

            bool isInUse = await _context.Main_Tasks
                .AnyAsync(t => t.StatusId == id);

            if (isInUse)
                return BadRequest(new { message = "Cannot delete: This status is currently in use by active tasks." });

            _context.Statuses.Remove(status);
            await _context.SaveChangesAsync();
            return Ok(new { message = "Status deleted" });
        }
    }
}
