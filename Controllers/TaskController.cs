using Azure.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using QtechOJT_Net9.Database;
using QtechOJT_Net9.DTO.MainTask;
using QtechOJT_Net9.DTO.Phase;
using QtechOJT_Net9.DTO.Severity;
using QtechOJT_Net9.DTO.Status;
using QtechOJT_Net9.DTO.SubTask;
using QtechOJT_Net9.DTO.User;
using QtechOJT_Net9.Models;
using System;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;

namespace QtechOJT_Net9.Controllers
{


    // DO NOTE that ASP.Net routes are -=-=CASE INSENSITIVE=-=-
    [Route("api/[controller]s")]
    [ApiController]
    public class TaskController(KanbanDbContext context) : ControllerBase
    {
        // Underscore appended to prefix is a general OOP practice to mean =
        //   "this variable is only for this object/file/namespace", examples are readonly variables like below
        private readonly KanbanDbContext _context = context;


        // Private foo for counting the Mandays excluding Sat and Sun
        private static int CountMandays(DateTime StartDate, DateTime TargetDate)
        {
            int count = 0;
            for (var d = StartDate.Date.AddDays(1); d <= TargetDate.Date; d = d.AddDays(1) ) // AddDays(1) to the CreatedAt to accurately get the exact count
            {
                if (d.DayOfWeek != DayOfWeek.Saturday && d.DayOfWeek != DayOfWeek.Sunday) // LINQ methods calling DayOfWeek
                    count++;
            }
            return count;
        }

        // A bit buggy, reiterate later?? It causes a cartesian explosion
        // for now, removed all references to this function
        //   Using it as a template to return a complete Task JSON with an appended sub task array
        //async Task<ActionResult<Main_TaskDto>> GetMain_TaskById(int getId)
        //{
        //    var task = await _context.Main_Tasks
        //        .Where(m => m.Id == getId)
        //         .AsNoTracking()
        //        .Select(m => new Main_TaskDto(
        //                // General info
        //                m.Id, m.ProjectId,
        //                m.Title, m.Description, m.Progress,
        //                // Date info
        //                m.UpdatedAt, m.CreatedAt, m.ActualEndDate, m.TargetDate,
        //                // User info
        //                m.Assignee.Id, m.Assignee.Name, m.Assignee.Username,
        //                m.QaAssignee.Id, m.QaAssignee.Name, m.QaAssignee.Username,
        //                // Phase info
        //                m.Phase.Id, m.Phase.IsFinal, m.Phase.IsDefault, m.Phase.Grouping,
        //                // Severity and Status info
        //                m.Severity.Id, m.Severity.Label,
        //                m.Status.Id, m.Status.Label,
        //                // Variance counting
        //                m.Variance,
        //                // List of subtasks for that main_task
        //                m.Subtasks.Select(s => new GetSubTaskDto(
        //                        s.Id,
        //                        s.Title,
        //                        s.IsDone,
        //                        s.Main_TaskId
        //                        )
        //                    ).ToList()
        //                )
        //            ).FirstOrDefaultAsync();

        //    if (task is null)
        //    {
        //        return BadRequest();
        //    }
        //    return task;
        //}


        // IMPORTANT -- SCALAR cannot identify nullable path parameters
        [HttpGet]
        public async Task<IActionResult> GetGeneralMain_Tasks(
            [FromQuery] int? ProjectId, 
            [FromQuery] int? PhaseId, 
            [FromQuery] int? StatusId,
            [FromQuery] int? assignedUserId, 
            [FromQuery] string? grouping     )
            {
            var query = _context.Main_Tasks
                .AsQueryable();

            // BE VERY CAREFUL WITH FromQuery VARIABLES
            // It has to be EXACTTT with what the frontend responds with
            // IF and only IF the request has queries with these
            // Ideally, only PM/ADMIN roles will send these URL queries
            if (ProjectId.HasValue)
                query = query.Where(t => t.ProjectId == ProjectId.Value);
            if (StatusId.HasValue)
                query = query.Where(t => t.StatusId == StatusId.Value);
            if (PhaseId.HasValue)
                query = query.Where(t => t.PhaseId == PhaseId.Value);

            // OR SQL conditional for assignedUserId
            // search through the tasks 
            // Only DEVs/QA will send these URL queries
            if (assignedUserId.HasValue)
                query = query.Where(t =>
                    t.AssigneeId == assignedUserId.Value ||
                    t.QaAssigneeId == assignedUserId.Value);

            // grouping filter joins through Phase
            if (!string.IsNullOrEmpty(grouping))
                query = query.Where(t => t.Phase.Grouping == grouping);

            // EFCore abstraction
            //  if we call .AsQueryable, and it has exactly named variables
            //    Ex. FromQuery ProjectId instead of FromQuery getProjectId
            //  and if we await/fetch using the queryable
            //  .Select will automatically use the queries passed by the FE
            var tasks = await query
                .AsNoTracking()
                .Select(m => new GeneralFull_TaskByIdDto(
                        // General info
                        m.Id, m.ProjectId,
                        m.Title, m.Description, m.Progress,
                        // Date info
                        m.UpdatedAt, m.CreatedAt, m.ActualEndDate, m.StartDate, m.TargetDate,
                        // User info
                        m.Assignee.Id, m.Assignee.Name,
                        m.QaAssignee.Id, m.QaAssignee.Name,

                        // Status info
                        m.Status.Id, m.Status.Label, m.Status.Color,
                        // Severity info
                        m.Severity.Id, m.Severity.Label, m.Severity.Color, m.Severity.SortOrder,
                        // Phase info
                        m.Phase.Id, m.Phase.Label, m.Phase.Grouping,
                        m.Variance,
                        m.Mandays,
                        m.Subtasks.Select(s => new GetSubTaskDto(
                                s.Id,
                                s.Title,
                                s.IsDone,
                                s.Main_TaskId
                                )
                            ).ToList()
                        )
                    ).ToListAsync();

            return Ok(tasks);
        }


        [HttpGet("/{getId}")]
        public async Task<ActionResult<GeneralMain_TaskByIdDto>> ParseMain_TaskById(int getId)
        {
            //We're getting a list type/mapped object, so we use var and let the abstraction do the work
            // This is the first implementation of a fully mapped out Task object,
            //   so I sectioned the model/entity variables for future readability
            //   this is a parameterized object, so it HAS TO BE IN ORDER AS DECLARED IN THE OBJECT FILE
            var task = await _context.Main_Tasks
                .AsNoTracking()
                .Where(m => m.Id == getId)
                .Select(m => new GeneralFull_TaskByIdDto(
                        // General info
                        m.Id, m.ProjectId,
                        m.Title, m.Description, m.Progress,

                        // Date info
                        m.UpdatedAt, m.CreatedAt, m.ActualEndDate, m.StartDate, m.TargetDate,

                        // User info
                        m.Assignee.Id, m.Assignee.Name, 
                        m.QaAssignee.Id, m.QaAssignee.Name,

                        // Status info
                        m.Status.Id, m.Status.Label, m.Status.Color,

                        // Severity info
                        m.Severity.Id, m.Severity.Label, m.Severity.Color, m.Severity.SortOrder,

                        // Phase info
                        m.Phase.Id, m.Phase.Label, m.Phase.Grouping,
                        // Variance
                        m.Variance,
                        m.Mandays,
                        m.Subtasks.Select(s => new GetSubTaskDto(
                                s.Id,
                                s.Title,
                                s.IsDone,
                                s.Main_TaskId
                                )
                            ).ToList()
                        )
                    ).FirstOrDefaultAsync();

            if (task is null)
                return NotFound();

            return Ok(task);
        }


        [HttpPost]
        public async Task<ActionResult<PostMain_TaskDto>> AddMain_Task(PostMain_TaskDto req)
        {
            // We will attempt to read from the Header; Frontend should have a login flow setup so that whoever sends the header, has a userId (from PM/Admin)
            // This is for logging later
            int? AssigneeId = null;
            if (Request.Headers.TryGetValue("x-user-id", out var AssigneeIdHeader))
                if (int.TryParse(AssigneeIdHeader, out var parsedId))
                    AssigneeId = parsedId;

            if (req is null)
                return BadRequest("Body is null");

            if (req.Title.IsNullOrEmpty())
                return BadRequest("Title is null");

            var PostDesc = "";
            if (!req.Description.IsNullOrEmpty())
                PostDesc = req.Description.Trim();
            else
            {
                PostDesc = "";
            }

            var ResolvedSeverityId = req.SeverityId;
            if (req.SeverityId is null)
                ResolvedSeverityId = await _context.Severities
                    .MaxAsync(s => s.SortOrder);

            // Guards for TargetDate, StartDate, ProjectId, and PhaseId
            if (req.TargetDate < DateTime.Now || req.TargetDate < req.StartDate)
                return BadRequest("Target Date must be a valid future date.");

            if (req.TargetDate is null)
                req.TargetDate = DateTime.Now;

            if (req.StartDate < DateTime.Now.Date || req.StartDate > req.TargetDate)
                return BadRequest("Start Date must be a valid date.");

            if (req.StartDate is null)
                req.StartDate = DateTime.Now;


            if (req.ProjectId == 0)
                return BadRequest("No ID");



            var ResolvedPhaseId = req.PhaseId;
            if (ResolvedPhaseId == 0)
            {
                // Call from the DB and select the first IsDefault column with a value of 1, if PhaseId is NULL or 0
                //   Later on, maybe, add another constraint that lets us pick between default values based on user role (Assignee, QaAssignee, PM)
                var defaultPhase = await _context.Phases
                    .Where(p => p.IsDefault == 1)
                    .FirstOrDefaultAsync();

                if (defaultPhase == null)
                    return StatusCode(500, new { message = "No default phase configured." });

                ResolvedPhaseId = defaultPhase.Id;
            }
            
            var task = new Main_Task
            {
                Title = req.Title.Trim(),
                Description = PostDesc,
                Progress = 0,
                ProjectId = req.ProjectId,

                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
                StartDate = (DateTime)req.StartDate,
                TargetDate = (DateTime)req.TargetDate, // casting to remove nullable ambiguity

                SeverityId = ResolvedSeverityId,
                PhaseId = ResolvedPhaseId,
                StatusId = req.StatusId,
                AssigneeId = req.AssigneeId,
                QaAssigneeId = req.QaAssigneeId,
                Variance = null,
                Mandays = CountMandays((DateTime)req.StartDate, (DateTime)req.TargetDate)

            };

            _context.Main_Tasks.Add(task);
            await _context.SaveChangesAsync();
            // newMain_Task.Id = exampleTasks.Max(t => t.Id) + 1;
            // RESTful practice, using CreatedAtAction which will return a 201

            var activity = new Activity
            {
                Main_TaskId = task.Id,
                UserId = AssigneeId,
                ActionDone = "Task created",
                CreatedAt = DateTime.Now
            };

            _context.Activity_Log.Add(activity);
            await _context.SaveChangesAsync();

            //return CreatedAtAction(nameof(ParseMain_TaskById), new { getId = task.Id }, task); // Somehow this returns a cartesian explosion.
            var created = await _context.Main_Tasks
                .AsNoTracking()
                .Where(m => m.Id == task.Id)
                .Select(m => new GeneralMain_TaskByIdDto(
                    m.Id, m.ProjectId,
                    m.Title, m.Description, m.Progress,
                    m.UpdatedAt, m.CreatedAt, m.ActualEndDate, m.StartDate, m.TargetDate,
                    m.Assignee.Id, m.Assignee.Name,
                    m.QaAssignee.Id, m.QaAssignee.Name,
                    m.Status.Id, m.Status.Label,
                    m.Severity.Id, m.Severity.Label, m.Severity.Color, m.Severity.SortOrder,
                    m.Phase.Id, m.Phase.Label, m.Phase.Grouping,
                    m.Variance,
                    m.Mandays
                ))
                .FirstOrDefaultAsync();

            return Ok(created);
        }



        [HttpPatch("{getId}/phase")]
        public async Task<IActionResult> ChangePhase(int getId, ChangePhaseMain_TaskDto req)
        {
            //const { id }                     = req.params;
            //const { phaseId, actualEndDate } = req.body;
            //const userId = req.headers["x-user-id"] ? Number(req.headers["x-user-id"]) : null;

            int? AssigneeId = null;
            if (Request.Headers.TryGetValue("x-user-id", out var AssigneeIdHeader))
                if (int.TryParse(AssigneeIdHeader, out var parsedId))
                    AssigneeId = parsedId;

            if (req.PhaseId is 0)
                return BadRequest("PhaseId is required.");

            var task = await _context.Main_Tasks
                .Where(m => m.Id == getId)
                .FirstOrDefaultAsync();

            if (task is null)
                return BadRequest("Task not found");

            var oldPhase = await _context.Phases  // Store the Old Phase for activity logging
                .Where(p => p.Id == task.PhaseId)
                .FirstOrDefaultAsync();

            var newPhase = await _context.Phases
                .Where(p => p.Id == req.PhaseId) // Find the phaseId inside the Phases table
                .FirstOrDefaultAsync(); //store the first result into the phase variable

            if (newPhase is null)
                return BadRequest("Phase not found");

            task.PhaseId = newPhase.Id;


            DateOnly realEnd = DateOnly.FromDateTime(DateTime.UtcNow);

            if (newPhase.IsFinal == 1)
            {
                task.ActualEndDate = req.ActualEndDate;
                task.Progress = 100;

                task.Variance = req.ActualEndDate.HasValue
                ? (int)(req.ActualEndDate.Value.Date - task.TargetDate.Date).TotalDays
                : null;

            }
            await _context.SaveChangesAsync();

            var logAction = $"Phase changed from \"{oldPhase?.Label}\" to \"{newPhase.Label}\""
                            + (newPhase.IsFinal == 1 && task.ActualEndDate.HasValue
                            ? $" — Actual End Date: {task.ActualEndDate.Value:yyyy-MM-dd}" : "");

            _context.Activity_Log.Add(new Activity
            {
                Main_TaskId = getId,
                UserId = AssigneeId ?? 0,
                ActionDone = logAction,
                CreatedAt = DateTime.Now
            });

            await _context.SaveChangesAsync(); // Two save changes
            var updated = await _context.Main_Tasks
                 .AsNoTracking()
                .Where(m => m.Id == task.Id)
                .Select(m => new GeneralMain_TaskByIdDto(
                    m.Id, m.ProjectId,
                    m.Title, m.Description, m.Progress,
                    m.UpdatedAt, m.CreatedAt, m.ActualEndDate, m.StartDate, m.TargetDate,
                    m.Assignee.Id, m.Assignee.Name,
                    m.QaAssignee.Id, m.QaAssignee.Name,
                    m.Status.Id, m.Status.Label,
                    m.Severity.Id, m.Severity.Label, m.Severity.Color, m.Severity.SortOrder,
                    m.Phase.Id, m.Phase.Label, m.Phase.Grouping,
                    m.Variance,
                    m.Mandays
                ))
                .FirstOrDefaultAsync();

            return Ok(updated);

        }



        [HttpPatch("{getId}/subtasks")]
        public async Task<IActionResult> UpdateSubtasks(int getId, [FromBody] UpdateSubtasksDto dto)
        {
            var task = await _context.Main_Tasks
                .Include(t => t.Subtasks)
                .FirstOrDefaultAsync(t => t.Id == getId);

            if (task == null)
                return NotFound(new { message = "Task not found" });

            // This is an OPTIMISTIC update
            var incomingIds = dto.Subtasks
                .Where(s => s.Id != 0)
                .Select(s => s.Id)
                .ToHashSet();

            // Only delete subtasks that were actually removed by the user
            var toDelete = task.Subtasks
                .Where(s => !incomingIds.Contains(s.Id))
                .ToList();

            await _context.Sub_Tasks
                .Where(s => toDelete.Select(d => d.Id).Contains(s.Id))
                .ExecuteDeleteAsync();

            // Update existing subtasks in-place (preserves their comments)
            foreach (var existing in task.Subtasks.Where(s => incomingIds.Contains(s.Id)))
            {
                var incoming = dto.Subtasks.First(s => s.Id == existing.Id);
                existing.Title = incoming.Title;
                existing.IsDone = incoming.IsDone ? 1 : 0;
            }

            // Add brand new subtask (Id == 0)
            var newSubtasks = dto.Subtasks
                .Where(s => s.Id == 0)
                .Select(s => new Sub_Task
                {
                    Title = s.Title,
                    IsDone = s.IsDone ? 1 : 0,
                    Main_TaskId = getId
                }).ToList();

            _context.Sub_Tasks.AddRange(newSubtasks);

            // Recalculate progress across the final set
            var allSubtasks = task.Subtasks
                .Where(s => incomingIds.Contains(s.Id))
                .Concat(newSubtasks)
                .ToList();

            var total = allSubtasks.Count;
            var done = allSubtasks.Count(s => s.IsDone == 1);
            task.Progress = total > 0 ? (int)Math.Round((double)done / total * 100) : 0;

            await _context.SaveChangesAsync();

            var updated = await _context.Main_Tasks
                 .AsNoTracking()
                .Where(m => m.Id == task.Id)
                .Select(m => new GeneralFull_TaskByIdDto(
                    m.Id, m.ProjectId,
                    m.Title, m.Description, m.Progress,
                    m.UpdatedAt, m.CreatedAt, m.ActualEndDate, m.StartDate, m.TargetDate,
                    m.Assignee.Id, m.Assignee.Name,
                    m.QaAssignee.Id, m.QaAssignee.Name,
                    m.Status.Id, m.Status.Label, m.Status.Color,
                    m.Severity.Id, m.Severity.Label, m.Severity.Color, m.Severity.SortOrder,
                    m.Phase.Id, m.Phase.Label, m.Phase.Grouping,
                    m.Variance,
                    m.Mandays,
                    m.Subtasks.Select(s => new GetSubTaskDto(
                            s.Id, 
                            s.Title, 
                            s.IsDone, 
                            s.Main_TaskId)
                        ).ToList()
                    )).FirstOrDefaultAsync();

            return Ok(updated);
        }

        // Update the entire task
        [HttpPut("{getId}")]
        public async Task<IActionResult> UpdateMain_Task(int getId, [FromBody] UpdateMain_TaskDto req)
        {
            int? AssigneeId = null;
            if (Request.Headers.TryGetValue("x-user-id", out var AssigneeIdHeader))
                if (int.TryParse(AssigneeIdHeader, out var parsedId))
                    AssigneeId = parsedId;

            var task = await _context.Main_Tasks
               .Include(t => t.Assignee)
               .Include(t => t.QaAssignee)
               .Include(t => t.Status)
               .Include(t => t.Severity)
               .FirstOrDefaultAsync(t => t.Id == getId);

            if (task == null)
                return NotFound(new { message = "Task not found" });

            // Guards for TargetDate, StartDate
            if (req.TargetDate < DateTime.Now || req.TargetDate < req.StartDate)
                return BadRequest("Target Date must be a valid future date and cannot be before the Start Date.");

            if (req.TargetDate is null)
                req.TargetDate = DateTime.Now;


            if (req.StartDate < DateTime.Now.Date || req.StartDate > req.TargetDate)
                return BadRequest("Start Date must be a valid past date and cannot be after the Target Date.");

            if (req.StartDate is null)
                req.StartDate = DateTime.Now;

            // Snapshot prev values before overwriting (for activity log later)
            var prev = new
            {
                task.Title,
                task.Description,
                task.TargetDate,
                task.AssigneeId,
                AssigneeName = task.Assignee?.Name,
                task.QaAssigneeId,
                QaAssigneeName = task.QaAssignee?.Name,
                task.StatusId,
                StatusLabel = task.Status?.Label,
                task.SeverityId,
                SeverityLabel = task.Severity?.Label,
            };

            // Apply a full update
            task.Title = req.Title;
            task.Description = req.Description;
            task.StatusId = req.StatusId ?? task.StatusId; //if request body field is null, revert to previous statusid
            task.SeverityId = req.SeverityId ?? task.SeverityId;
            task.AssigneeId = req.AssigneeId;
            task.QaAssigneeId = req.QaAssigneeId;
            task.StartDate = req.StartDate ?? DateTime.Now; // if start date is null, set to now, otherwise, set to the requested start date
            task.TargetDate = req.TargetDate ?? task.TargetDate;
            task.Mandays = CountMandays((DateTime)task.StartDate, (DateTime)task.TargetDate);
            task.UpdatedAt = DateTime.Now;

            // EFCore tracks all of the changes of a specific instantiated Model when we fetch something from the DBContext
            //   so here we fetched a task from the Main_Task table, mutate it, and then save the changes in the next calling line of code.

            await _context.SaveChangesAsync();

            // Again, double saving for a more iterative approach


            // ── Build specific log messages ────────────────────────
            var logs = new List<string>();

            if (req.Title != prev.Title)
                logs.Add($"Title changed from \"{prev.Title}\" to \"{req.Title}\"");

            if (req.Description != prev.Description)
                logs.Add("Description updated");

            if (req.TargetDate.HasValue && req.TargetDate.Value != prev.TargetDate)
                logs.Add($"Target date changed from \"{prev.TargetDate:yyyy-MM-dd}\" to \"{req.TargetDate.Value:yyyy-MM-dd}\"");

            if (req.StartDate != task.StartDate)
                logs.Add(req.StartDate.HasValue
                    ? $"Start date set to \"{req.StartDate.Value:yyyy-MM-dd}\""
                    : "Start date cleared");


            if (req.AssigneeId != prev.AssigneeId)
            {
                var newName = req.AssigneeId.HasValue
                    ? await _context.Users
                        .Where(u => u.Id == req.AssigneeId)
                        .Select(u => u.Name)
                        .FirstOrDefaultAsync() ?? "Unknown"
                    : "Unassigned";
                logs.Add($"Assignee changed from \"{prev.AssigneeName ?? "Unassigned"}\" to \"{newName}\"");
            }

            if (req.QaAssigneeId != prev.QaAssigneeId)
            {
                var newName = req.QaAssigneeId.HasValue // okay, so EFCore NEEDS to be very precise with nullability
                    // Members that are not nullable do not have the .HasValue function
                    ? await _context.Users
                        .Where(u => u.Id == req.QaAssigneeId)
                        .Select(u => u.Name)
                        .FirstOrDefaultAsync() ?? "Unknown"
                    : "Unassigned";
                logs.Add($"QA Assignee changed from \"{prev.QaAssigneeName ?? "Unassigned"}\" to \"{newName}\"");
            }

            if (req.StatusId.HasValue && req.StatusId != prev.StatusId)
            {
                var newLabel = await _context.Statuses
                    .Where(s => s.Id == req.StatusId)
                    .Select(s => s.Label)
                    .FirstOrDefaultAsync();
                logs.Add($"Status changed from \"{prev.StatusLabel}\" to \"{newLabel}\"");
            }

            if (req.SeverityId.HasValue && req.SeverityId != prev.SeverityId)
            {
                var newLabel = await _context.Severities
                    .Where(s => s.Id == req.SeverityId)
                    .Select(s => s.Label)
                    .FirstOrDefaultAsync();
                logs.Add($"Severity changed from \"{prev.SeverityLabel}\" to \"{newLabel}\"");
            }

            if (logs.Count > 0 && AssigneeId.HasValue)
            {
                _context.Activity_Log.AddRange(logs.Select(log => new Activity
                {
                    Main_TaskId = getId,
                    UserId = AssigneeId.Value,
                    ActionDone = log,
                    CreatedAt = DateTime.Now
                }));
                await _context.SaveChangesAsync();
            }
            var updated = await _context.Main_Tasks
               .AsNoTracking()
               .Where(m => m.Id == task.Id)
               .Select(m => new GeneralFull_TaskByIdDto(
                   m.Id, m.ProjectId,
                   m.Title, m.Description, m.Progress,
                   m.UpdatedAt, m.CreatedAt, m.ActualEndDate, m.StartDate, m.TargetDate,
                   m.Assignee.Id, m.Assignee.Name,
                   m.QaAssignee.Id, m.QaAssignee.Name,
                   m.Status.Id, m.Status.Label, m.Status.Color,
                   m.Severity.Id, m.Severity.Label, m.Severity.Color, m.Severity.SortOrder,
                   m.Phase.Id, m.Phase.Label, m.Phase.Grouping,
                   m.Variance,
                   m.Mandays,
                   m.Subtasks.Select(s => new GetSubTaskDto(
                               s.Id,
                               s.Title,
                               s.IsDone,
                               s.Main_TaskId
                               )
                           ).ToList()
                   )
               ).FirstOrDefaultAsync();


            return Ok(updated);
        }


        // -- PATCH for progress 0 to 100 ------------------------------
        // Directly set progress (used when a task has no subtasks)
        [HttpPatch("{getId}/progress")]
        public async Task<IActionResult> UpdateProgress(int getId, [FromBody] UpdateProgressDto req)
        {
            if (req.Progress < 0 || req.Progress > 100)
                return BadRequest(new { message = "Progress must be between 0 and 100." });

            var task = await _context.Main_Tasks.FindAsync(getId);
            if (task is null) return NotFound(new { message = "Task not found." });

            task.Progress = req.Progress;
            task.UpdatedAt = DateTime.Now;
            await _context.SaveChangesAsync();

            return Ok(new { task.Id, task.Progress });
        }

        // Delete
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMainTask(int id)
        {
            var task = await _context.Main_Tasks.FindAsync(id);

            if (task is null)
                return NotFound(new { message = $"Task with ID {id} not found." });

            _context.Main_Tasks.Remove(task);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Task deleted." });
        }


    }
}
