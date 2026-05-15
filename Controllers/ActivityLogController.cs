using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QtechOJT_Net9.Database;

namespace QtechOJT_Net9.Controllers
{
    [Route("api/[controller]s")]
    [ApiController]
    public class ActivityLogController(KanbanDbContext context) : ControllerBase
    {
        private readonly KanbanDbContext _context = context;

        // TODO:
        //   Rework entire activity logging logic
        //      Ideally, I think, the best pattern is to just throw the Title and the User's name
        //      This will make it so that we no longer rely on broken FK interactions with the Main_Tasks and Users tables

        // -- GET api/activityLogs?taskId=&userId=&from=&to= ------------------------
        [HttpGet]
        public async Task<IActionResult> GetActivityLogs(
            [FromQuery] int? taskId,
            [FromQuery] int? userId,
            [FromQuery] DateTime? from,
            [FromQuery] DateTime? to,
            [FromQuery] int page = 1,
            [FromQuery] int limit = 10 )
        {
            var query = _context.Activity_Log.AsQueryable();

            if (taskId.HasValue)
                query = query.Where(a => a.Main_TaskId == taskId.Value);

            if (userId.HasValue)
                query = query.Where(a => a.UserId == userId.Value);

            if (from.HasValue)
                query = query.Where(a => a.CreatedAt >= from.Value);

            // JS appends " 23:59:59" to the `to` date — mirror that by going to end of day
            // to mirror it, we just add a day then subtract 1 second from it
            if (to.HasValue)
                query = query.Where(a => a.CreatedAt <= to.Value.Date.AddDays(1).AddTicks(-1));

            // This is the base query that we will use for selecting.
            var baseQuery = query.AsNoTracking();

            // We get the pure total of all the rows
            var total = await baseQuery.CountAsync();

            var data = await baseQuery
                .AsNoTracking()
                .OrderByDescending(a => a.CreatedAt)
                .Skip((page - 1) * limit)
                .Take(limit) // LINQ for limiting the Query, from whatever the url has sent
                .Select(a => new
                {
                    a.Id,
                    Action = a.ActionDone,
                    a.CreatedAt,
                    TaskId = a.Main_TaskId,
                    TaskTitle = a.Main_Task.Title,
                    UserId = a.UserId,
                    UserName = a.User.Name,
                    UserRole = a.User.Role,
                })
                .ToListAsync();
                

            return Ok(new
            {
                data,
                total,
                page,
                limit,
                totalPages = (int)Math.Ceiling((double)total/limit),
            });
        }
    }
}
