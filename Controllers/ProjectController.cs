using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using QtechOJT_Net9.Database;
using QtechOJT_Net9.Models;
using QtechOJT_Net9.DTO.MainTask;
using QtechOJT_Net9.DTO.Phase;
using QtechOJT_Net9.DTO.Severity;
using QtechOJT_Net9.DTO.Status;
using QtechOJT_Net9.DTO.SubTask;
using QtechOJT_Net9.DTO.User;
using QtechOJT_Net9.DTO.Project;

namespace QtechOJT_Net9.Controllers
{
    [Route("api/[controller]s")]
    [ApiController]
    public class ProjectController(KanbanDbContext context) : ControllerBase
    {
        private readonly KanbanDbContext _context = context;

        private Task<ProjectResponseDto?> GetProjectByIdAsync(int id) => _context.Projects
                .AsNoTracking()
                .Where(p => p.Id == id)
                .Select(p => new ProjectResponseDto
                {
                    Id = p.Id,
                    Title = p.Title,
                    Description = p.Description,
                    ClientName = p.ClientName,
                    TargetEndDate = p.TargetEndDate,
                    Status = p.Status,
                    CreatedAt = p.CreatedDate,
                    PmId = p.PmId,
                    PmName = p.PM != null ? p.PM.Name : null,
                    PmUsername = p.PM != null ? p.PM.Username : null,
                    TaskCount = p.Main_Tasks.Count
                    //TaskCount = _context.Main_Tasks.Count(t => t.ProjectId == p.Id)
                })
                .FirstOrDefaultAsync();


        private async Task SyncProjectUsers(int projectId, List<int> userIds, string role)
        {
            var existing = await _context.Project_Users
                .Where(pu => pu.ProjectId == projectId && pu.Role == role) // find the Project AND the compatible role
                //.ToListAsync();
                .ExecuteDeleteAsync();

            //_context.Project_Users.RemoveRange(existing); 
            // Commented out kasi there was an issue with the entity tracker and changes are not being saved immediately

            _context.Project_Users.AddRange(userIds.Select(uid => new ProjectUser
                {
                    ProjectId = projectId,
                    UserId = uid,
                    Role = role
                }));
        }


        // -- GET api/projects ------------------------
        [HttpGet]
        public async Task<IActionResult> GetProjects()
        {
            var role = Request.Headers.TryGetValue("x-user-role", out var roleVal)
                ? roleVal.ToString() : null;

            var isPM = role == "ProjectManager";
            var isAdmin = role == "Admin";

            if (!isPM && !isAdmin)
                return StatusCode(403, new { message = "Access denied" });

            // Only parse userId when we actually need it (PM filter)
            int? userId = null;
            if (isPM &&
                Request.Headers.TryGetValue("x-user-id", out var userIdStr) &&
                int.TryParse(userIdStr, out var parsedId))
            {
                userId = parsedId;
            }

            var projects = await _context.Projects
                .AsNoTracking()
                .Where(p => !isPM || p.PmId == userId)
                .OrderByDescending(p => p.CreatedDate)
                .Select(p => new ProjectResponseDto
                {
                    Id = p.Id,
                    Title = p.Title,
                    Description = p.Description,
                    ClientName = p.ClientName,
                    TargetEndDate = p.TargetEndDate,
                    Status = p.Status,
                    CreatedAt = p.CreatedDate,
                    PmId = p.PmId,
                    PmName = p.PM != null ? p.PM.Name : null, // Inline PM filter: when isPM is true the WHERE clause is applied, ternary operator
                    PmUsername = p.PM != null ? p.PM.Username : null,

                    // EF Core translates this to a SQL COUNT subquery,
                    // equivalent to the JS LEFT JOIN + COUNT(t.id) but EFCore already joins without explicit Includes .
                    TaskCount = _context.Main_Tasks.Count(t => t.ProjectId == p.Id)
                })
                .ToListAsync();

            return Ok(projects);
        }


        // -- POST /api/projects ------------------------
        public async Task<IActionResult> CreateProject( [FromBody] ProjectRequestDto req )
        {
            if (string.IsNullOrWhiteSpace(req.Title))
                return BadRequest(new { message = "Title is required" });

            // Todo add guard against setting a date from before .Now
            if (req.TargetEndDate < DateTime.Now)
                return BadRequest("Please enter a finish date that is valid.");

            var project = new Project
            {
                Title = req.Title.Trim(),
                Description = req.Description,
                PmId = req.PmId,
                ClientName = req.ClientName?.Trim() ?? string.Empty,
                TargetEndDate = req.TargetEndDate ?? DateTime.Now,
                Status = req.Status ?? "ongoing",
                CreatedDate = DateTime.Now
            };

            _context.Projects.Add(project);

            try
            {
                await _context.SaveChangesAsync();

                await SyncProjectUsers(project.Id, req.Developers, "Developer");
                await _context.SaveChangesAsync();
                await SyncProjectUsers(project.Id, req.Qas, "QA");
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex) // I had Claude do a try-catch to get if there's a duplicate key, maybe there's a more elegant way to do this without raw try-catch
                when (ex.InnerException?.Message.Contains("UNIQUE", StringComparison.OrdinalIgnoreCase) == true ||
                      ex.InnerException?.Message.Contains("duplicate key", StringComparison.OrdinalIgnoreCase) == true)
                {
                return Conflict(new { message = "Project title already exists" });
                }

            var response = await GetProjectByIdAsync(project.Id);
            return CreatedAtAction(nameof(GetProjects), new { id = project.Id }, response);
        }


        // -- PUT /api/projects/:id ------------------------
        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateProject(
            int id, [FromBody] ProjectRequestDto req)
        {
            if (string.IsNullOrWhiteSpace(req.Title))
                return BadRequest(new { message = "Title is required" });

            // FindAsync is preferred over FirstOrDefaultAsync when looking up by PK —
            // it checks the change tracker first before hitting the DB.
            var project = await _context.Projects.FindAsync([id]);
            if (project is null)
                return NotFound(new { message = "Project not found" });

            project.Title = req.Title.Trim();
            project.Description = req.Description;
            project.PmId = req.PmId;
            project.ClientName = req.ClientName?.Trim() ?? project.ClientName;
            project.TargetEndDate = req.TargetEndDate ?? project.TargetEndDate;
            project.Status = req.Status ?? "ongoing";

            try
            {
                await _context.SaveChangesAsync();

                await SyncProjectUsers(id, req.Developers, "Developer");
                await SyncProjectUsers(id, req.Qas, "QA");
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
                when (ex.InnerException?.Message.Contains("UNIQUE", StringComparison.OrdinalIgnoreCase) == true ||
                      ex.InnerException?.Message.Contains("duplicate key", StringComparison.OrdinalIgnoreCase) == true)
            {
                return Conflict(new { message = "Project title already exists" });
            }

            var response = await GetProjectByIdAsync(id);
            return Ok(response);
        }


        // -- DELETE /api/projects/:id ------------------------
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteProject(int id, CancellationToken ct)
        {
            var project = await _context.Projects.FindAsync([id], ct);
            if (project is null)
                return NotFound(new { message = "Project not found" });

            _context.Projects.Remove(project);
            await _context.SaveChangesAsync(ct);

            return Ok(new { message = "Project deleted" });
        }


        // -- GET /api/projects/my ------------------------
        // — Dev/QA: projects I'm assigned to via project_users
        [HttpGet("my")]
        public async Task<IActionResult> GetMyProjects()
        {
            if (!Request.Headers.TryGetValue("x-user-id", out var userIdStr) ||
                !int.TryParse(userIdStr, out var userId))
                return BadRequest(new { message = "User not identified" });

            var projects = await _context.Project_Users
                .AsNoTracking()
                .Where(pu => pu.UserId == userId)
                .Select(pu => new ProjectResponseDto
                {
                    Id = pu.Project.Id,
                    Title = pu.Project.Title,
                    Description = pu.Project.Description,
                    ClientName = pu.Project.ClientName,
                    TargetEndDate = pu.Project.TargetEndDate,
                    Status = pu.Project.Status,
                    CreatedAt = pu.Project.CreatedDate,
                    PmId = pu.Project.PmId,
                    PmName = pu.Project.PM != null ? pu.Project.PM.Name : null,
                    TaskCount = _context.Main_Tasks.Count(t => t.ProjectId == pu.ProjectId)
                })
                .OrderBy(p => p.Title)
                .ToListAsync();

            return Ok(projects);
        }



    }
}
