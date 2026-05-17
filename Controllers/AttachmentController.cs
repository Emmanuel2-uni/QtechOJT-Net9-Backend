using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QtechOJT_Net9.Database;
using QtechOJT_Net9.Models;
using System.Threading.Tasks;


namespace QtechOJT_Net9.Controllers
{

    // The only pure vibe coded route
    //   I do not understand how to make use of the filestreams here in C#
    //    so I relied on a Claude project that I trained using Models and Questions regarding C#

    [Route("api/[controller]s")]
    [ApiController]
    public class AttachmentController(KanbanDbContext context, IWebHostEnvironment env) : ControllerBase
    {
        private readonly KanbanDbContext _context = context;

        private static readonly HashSet<string> AllowedExtensions = new(StringComparer.OrdinalIgnoreCase)
        {
            ".jpg", ".jpeg", ".png", ".gif", ".webp",
            ".pdf", ".xlsx", ".xls", ".csv",
            ".doc", ".docx", ".txt", ".zip"
        };
        private const long MaxFileSizeBytes = 10 * 1024 * 1024; // 10 MB

        // Mirrors multer's destination: uploads/<taskId>/
        private string TaskDir(int taskId) =>
            Path.Combine(env.ContentRootPath, "uploads", taskId.ToString());


        // GET api/attachments/{taskId}
        [HttpGet("{taskId}")]
        public async Task<IActionResult> GetAttachments(int taskId)
        {
            var attachments = await _context.Task_Attachments
                 .AsNoTracking()
                .Where(a => a.Main_TaskId == taskId)
                .OrderByDescending(a => a.UploadedAt)
                .ToListAsync();

            return Ok(attachments);
        }


        // POST api/attachments/{taskId}
        [HttpPost("{taskId}")]
        public async Task<IActionResult> Upload(int taskId, IFormFile? file)
        {
            if (file is null || file.Length == 0)
                return BadRequest(new { message = "No file received." });

            if (file.Length > MaxFileSizeBytes)
                return BadRequest(new { message = "Upload failed: File must be under 10MB and be a valid document/image type." });

            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!AllowedExtensions.Contains(ext))
                return BadRequest(new { message = $"Upload failed: \"{ext}\" files are not allowed." });

            // Mirrors multer's filename: <timestamp>-<random><ext>
            var storedName = $"{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}-{Random.Shared.Next(1_000_000)}{ext}";
            var dir = TaskDir(taskId);
            Directory.CreateDirectory(dir);
            var filePath = Path.Combine(dir, storedName);

            await using (var stream = System.IO.File.Create(filePath))
                await file.CopyToAsync(stream);

            var attachment = new Task_Attachment
            {
                Main_TaskId = taskId,
                OriginalName = file.FileName,
                Filename = storedName,
                Mimetype = file.ContentType,
                Size = (int)file.Length,
                UploadedAt = DateTime.UtcNow
            };

            _context.Task_Attachments.Add(attachment);
            _context.Activity_Log.Add(new Activity
            {
                Main_TaskId = taskId,
                ActionDone = $"File uploaded: {file.FileName}",
                CreatedAt = DateTime.UtcNow
            });

            try
            {
                await _context.SaveChangesAsync();
            }
            catch
            {
                System.IO.File.Delete(filePath); // Clean up if DB insert fails — same as JS
                return StatusCode(500, new { message = "Failed to save attachment metadata." });
            }

            return Created($"api/attachments/{taskId}", attachment);
        }


        // GET api/attachments/{taskId}/{attachmentId}/download
        [HttpGet("{taskId}/{attachmentId}/download")]
        public async Task<IActionResult> Download(int taskId, int attachmentId)
        {
            var attachment = await _context.Task_Attachments
                .FirstOrDefaultAsync(a => a.Id == attachmentId && a.Main_TaskId == taskId);

            if (attachment is null)
                return NotFound(new { message = "Attachment not found." });

            var filePath = Path.Combine(TaskDir(taskId), attachment.Filename);
            if (!System.IO.File.Exists(filePath))
                return NotFound(new { message = "File no longer exists on disk." });

            // File() streams the file — equivalent to fs.createReadStream().pipe(res)
            return File(System.IO.File.OpenRead(filePath), attachment.Mimetype, attachment.OriginalName);
        }


        // DELETE api/attachments/{taskId}/{attachmentId}
        [HttpDelete("{taskId}/{attachmentId}")]
        public async Task<IActionResult> Delete(int taskId, int attachmentId)
        {
            int? requestingId = null;
            if (Request.Headers.TryGetValue("x-user-id", out var AssigneeIdHeader))
                if (int.TryParse(AssigneeIdHeader, out var parsedId))
                    requestingId = parsedId;

            var attachment = await _context.Task_Attachments
                .FirstOrDefaultAsync(a => a.Id == attachmentId && a.Main_TaskId == taskId);

            if (attachment is null)
                return NotFound(new { message = "Attachment not found." });


            var requestingUser = await _context.Users.FindAsync(requestingId); // from [FromQuery] or body
            if (requestingUser is null) return Unauthorized();

            // GUARD for authorization: only the creator (PM) or Admin can delete the task
            // If NONE of these are true, then reject with 403:
            if ( requestingUser.Role != "ProjectManager" // Check if the requesting user is a PM
                    && requestingUser.Role != "Admin")     // Optionally, allow Admins too
                return StatusCode(403, new { message = "Only a PM or Admin can delete attachments." });



            var filePath = Path.Combine(TaskDir(taskId), attachment.Filename);

            _context.Task_Attachments.Remove(attachment);
            _context.Activity_Log.Add(new Activity
            {
                Main_TaskId = taskId,
                ActionDone = $"File deleted: {attachment.OriginalName}",
                CreatedAt = DateTime.UtcNow
            });

            await _context.SaveChangesAsync(); // DB first, same as JS

            // Disk delete after DB commit — don't throw if already missing
            if (System.IO.File.Exists(filePath))
                System.IO.File.Delete(filePath);

            return Ok(new { message = "Attachment deleted." });
        }
    }
}
