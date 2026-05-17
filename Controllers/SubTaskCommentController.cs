using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QtechOJT_Net9.Database;
using QtechOJT_Net9.DTO.SubTaskComments;
using QtechOJT_Net9.Models;

namespace QtechOJT_Net9.Controllers
{
    [Route("api/subtask-comments")] // We had to make a custom route so that it fits the frontend fetch requests
    [ApiController]
    public class SubTaskCommentController(KanbanDbContext context) : ControllerBase
    {
        private readonly KanbanDbContext _context = context;

        // A helper for a resuable, deferred query.
        // new technique/pattern here that we'll try to use (and not fail hopefully)
        //private IQueryable<CommentResponseDto> ProjectComments() =>
        //    _context.Sub_Task_Comments
        //        .AsNoTracking()
        //        .Select(c => new CommentResponseDto(
        //            c.Id,
        //            c.Sub_TaskId,
        //            c.UserId,
        //            c.Comment,
        //            c.CommentDate,
        //            c.User != null ? c.User.Name : null,
        //            c.User != null ? c.User.Username : null
        //        ));
        // technique failed, ignore lmao




        // -- GET /api/subtask-comments/:subtaskId --------------------
        [HttpGet("{subtaskId:int}")]
        public async Task<IActionResult> GetComments(int subtaskId)
        {
            var comments = await _context.Sub_Task_Comments
                .AsNoTracking()
                .Where(c => c.Sub_TaskId == subtaskId)
                .OrderBy(c => c.Id)
                .Select(c => new CommentResponseDto(
                    c.Id,
                    c.Sub_TaskId,
                    c.UserId,
                    c.Comment,
                    c.CommentDate,
                    c.User != null ? c.User.Name : null,
                    c.User != null ? c.User.Username : null
                    ))
                .ToListAsync();

            return Ok(comments);
        }





        // -- GET /api/subtask-comments/counts?subtaskIds=1,2,3 --------------------
        //    Returns a dictionary of { subtaskId: commentCount }
        [HttpGet("counts")]
        public async Task<IActionResult> GetCommentCounts([FromQuery] string subtaskIds)
        {
            // If there is no subtasks, return immediately
            if (string.IsNullOrWhiteSpace(subtaskIds))
                return Ok(new Dictionary<int, int>());


            // LINQ use where it is not JUST for ORM.
            // This declares a list of int
            var ids = subtaskIds
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(s => int.TryParse(s.Trim(), out var id) ? id 
                                                                : (int?)null) // Ternary
                .Where(id => id.HasValue)
                .Select(id => id!.Value)
                .ToList();

            var counts = await _context.Sub_Task_Comments
                .AsNoTracking()
                .Where(c => ids.Contains(c.Sub_TaskId))
                .GroupBy(c => c.Sub_TaskId)
                .Select(g => new { SubtaskId = g.Key, Count = g.Count() })
                .ToListAsync();

            // Fill in zeros for subtasks that have no comments yet
            var result = ids.ToDictionary(id => id, id => 0);

            foreach (var row in counts)
                result[row.SubtaskId] = row.Count;

            return Ok(result);
        }



        // -- POST /api/subtask-comments/:subtaskId --------------------
        [HttpPost("{subtaskId:int}")]
        public async Task<IActionResult> AddComment(int subtaskId, [FromBody] AddCommentDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Comment))
                return BadRequest(new { message = "Comment text is required" });

            int? userId = null;
            if (Request.Headers.TryGetValue("x-user-id", out var userIdHeader) // request the header of the user who is sending the comment
                && int.TryParse(userIdHeader, out var parsedId))
                userId = parsedId;

            if (userId is null)
                return BadRequest(new { message = "User not identified — x-user-id header missing" });

            // Guard flag, subtask must exist
            bool subtaskExists = await _context.Sub_Tasks.AnyAsync(s => s.Id == subtaskId);
            if (!subtaskExists)
                return NotFound(new { message = $"Subtask {subtaskId} not found" });

            var comment = new Sub_Task_Comment
            {
                Sub_TaskId = subtaskId,
                UserId = userId,
                Comment = dto.Comment.Trim(),
                CommentDate = DateTime.Now
            };

            _context.Sub_Task_Comments.Add(comment);
            await _context.SaveChangesAsync();

            // Return the newly created comment with commenter details
            //   Here we use the deferred query
            var created =  await _context.Sub_Task_Comments
                .AsNoTracking()
                .Where(c => c.Id == comment.Id)
                .Select(c => new CommentResponseDto(
                    c.Id,
                    c.Sub_TaskId,
                    c.UserId,
                    c.Comment,
                    c.CommentDate,
                    c.User != null ? c.User.Name : null,
                    c.User != null ? c.User.Username : null
                    ))
                .FirstOrDefaultAsync(); // ToLIstAsync() or FirstOrDefaultAsync no longer defers the private deferred query we defined

            return StatusCode(201, created);
        }

        [HttpPatch("{commentId:int}")]
        // -- PATCH /api/subtask-comments/:commentId --------------------
        public async Task<IActionResult> EditComment(int commentId, [FromBody] EditCommentDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Comment))
                return BadRequest(new { message = "Comment text is required" });

            int? userId = null; // get the current user ID from the request header, same as the add comment method
            if (Request.Headers.TryGetValue("x-user-id", out var userIdHeader)
                && int.TryParse(userIdHeader, out var parsedId))
                userId = parsedId;

            if (userId is null)
                return BadRequest(new { message = "User not identified — x-user-id header missing" });

            var comment = await _context.Sub_Task_Comments.FindAsync(commentId);
            if (comment is null)
                return NotFound(new { message = "Comment not found" });

            // Simple check to see if the current userId from the request header is the same as the comment ID
            if (comment.UserId != userId)
                return StatusCode(403, new { message = "You can only edit your own comments" });

            comment.Comment = dto.Comment.Trim();
            await _context.SaveChangesAsync();
            return Ok(new { comment = comment.Comment });
        }


        // -- DELETE /api/subtask-comments/:commentId --------------------
        //   Fires a delete method to the comment id
        [HttpDelete("{commentId:int}")]
        public async Task<IActionResult> DeleteComment(int commentId)
        {
            // Guard so that only the comment poster can delete the comment.
            int? requestingId = null;
            if (Request.Headers.TryGetValue("x-user-id", out var userIdHeader)
                && int.TryParse(userIdHeader, out var parsedId))
                requestingId = parsedId;

            var requestingUser = await _context.Users.FindAsync(requestingId); // from [FromQuery] or body
                if (requestingUser is null) return Unauthorized();

            if (requestingId is null)
                return BadRequest(new { message = "User not identified — x-user-id header missing" });

            var comment = await _context.Sub_Task_Comments.FindAsync(commentId);

            if (comment is null)
                return NotFound(new { message = "Comment not found" });

            // Simple check to see if the current userId from the request header is the same as the comment ID
            if (comment.UserId != requestingId
                && requestingUser.Role != "Admin")
                return StatusCode(403, new { message = "You can only delete your own comments, in addition to Admins" });

            _context.Sub_Task_Comments.Remove(comment);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Comment deleted" });
        }



    }
}
