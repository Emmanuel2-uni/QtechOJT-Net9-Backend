using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QtechOJT_Net9.Database;
using QtechOJT_Net9.DTO.SubTask;

namespace QtechOJT_Net9.Controllers
{
    [Route("api/subtasks")]
    [ApiController]
    public class SubTaskController(KanbanDbContext context) : ControllerBase
    {
        private readonly KanbanDbContext _context = context;

        // -- PATCH /api/subtasks/:id ----------------------
        [HttpPatch("{id:int}")]
        public async Task<IActionResult> EditSubTask(int id, [FromBody] EditSubTaskDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Title))
                return BadRequest(new { message = "Title is required" });

            int? userId = null;
            if (Request.Headers.TryGetValue("x-user-id", out var userIdHeader)
                && int.TryParse(userIdHeader, out var parsedId))
                userId = parsedId;

            if (userId is null)
                return BadRequest(new { message = "User not identified — x-user-id header missing" });

            var subtask = await _context.Sub_Tasks.FindAsync(id);
            if (subtask is null)
                return NotFound(new { message = "Subtask not found" });

            if (subtask.CreatorId != userId)
                return StatusCode(403, new { message = "You can only edit your own subtasks" });

            subtask.Title = dto.Title.Trim();
            await _context.SaveChangesAsync();

            return Ok(new { id = subtask.Id, title = subtask.Title });
        }
    }



}