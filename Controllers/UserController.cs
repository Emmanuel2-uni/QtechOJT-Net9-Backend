using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QtechOJT_Net9.Database;
using QtechOJT_Net9.DTO.User;
using QtechOJT_Net9.Models;

namespace QtechOJT_Net9.Controllers
{
    [Route("api/[controller]s")]
    [ApiController]
    public class UserController(KanbanDbContext context, IPasswordHasher<User> passwordHasher) : ControllerBase
    {
        private readonly KanbanDbContext _context = context;
        private readonly IPasswordHasher<User> _passwordHasher = passwordHasher; // Entity Framework's native hasher, no need for BCrypt
            
            // Private helper for sending to the requesting method/function
            private static UserResponse ToResponse(User u) =>
                new(u.Id, u.Name, u.Username, u.Role, u.IsActive);

            // ── GET /api/users ────────────────────────────────────────────
            // Public — active users only. Pass ?all=true to include inactive (Admin).
            [HttpGet]
            public async Task<IActionResult> GetUsers([FromQuery] bool all = false)
            {
                var users = await _context.Users
                    .AsNoTracking()
                    .Where(u => all || u.IsActive == 1)
                    .OrderBy(u => u.Name)
                    .Select(u => ToResponse(u))
                    .ToListAsync();

                return Ok(users);
            }

            [HttpGet("project/{getId}")]
            public async Task<IActionResult> GetUserByProject(int getId)
            {
                var users = await _context.Project_Users
                    .AsNoTracking()
                    .Where(p => p.ProjectId == getId)
                    .Select(p => new ProjectUserGetDto (
                        p.Id,
                        p.ProjectId,
                        p.UserId,
                        p.Role,
                        p.User.Name
                        ) 
                    ).ToListAsync();

                return Ok(users);

            }
            
            // -- POST /api/users --------------------------------
            // Admin only — create a new user.
            [HttpPost]
            public async Task<IActionResult> CreateUser([FromBody] CreateUserRequest body)
            {
                if (string.IsNullOrWhiteSpace(body.Name)) return BadRequest(new { message = "Name is required" });
                if (string.IsNullOrWhiteSpace(body.Username)) return BadRequest(new { message = "Username is required" });
                if (string.IsNullOrWhiteSpace(body.Password)) return BadRequest(new { message = "Password is required" });
                if (string.IsNullOrWhiteSpace(body.Role)) return BadRequest(new { message = "Role is required" });

                var normalizedUsername = body.Username.Trim().ToLower();

                if (await _context.Users.AnyAsync(u => u.Username == normalizedUsername))
                    return Conflict(new { message = "Username already exists" });

                // We hash the password first, since the next new "user" object does not exist yet,
                //  So the .HashPassword(object, var) will not be able to create an instance of the object because it references something that does not exist
                var hashedPassword = _passwordHasher.HashPassword(null!, body.Password);

                var user = new User
                {
                    Name = body.Name.Trim(),
                    Username = normalizedUsername,
                    Password = hashedPassword,
                    Role = body.Role,
                    IsActive = 1
                };

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetUsers), new { id = user.Id }, ToResponse(user)); // Entity Framework, upon success send the ID JSON at Id
            }

            // -- PUT /api/users/:id ---------------------------------
            // Admin only — update name, username, role.
            [HttpPut("{id}")]
            public async Task<IActionResult> UpdateUser(int id, [FromBody] UpdateUserRequest body)
            {
                if (string.IsNullOrWhiteSpace(body.Name)) return BadRequest(new { message = "Name is required" });
                if (string.IsNullOrWhiteSpace(body.Username)) return BadRequest(new { message = "Username is required" });
                if (string.IsNullOrWhiteSpace(body.Role)) return BadRequest(new { message = "Role is required" });

                var user = await _context.Users.FindAsync(id);
                if (user is null) return NotFound(new { message = "User not found" });

                var normalizedUsername = body.Username.Trim().ToLower();

                if (await _context.Users.AnyAsync(u => u.Username == normalizedUsername && u.Id != id))
                    return Conflict(new { message = "Username already exists" });

                user.Name = body.Name.Trim();
                user.Username = normalizedUsername;
                user.Role = body.Role;

                await _context.SaveChangesAsync();
                return Ok(ToResponse(user));
            }

            // -- PATCH /api/users/:id/password  ---------------------------------
            // Admin only — reset a user's password.
            [HttpPatch("{id}/password")]
            public async Task<IActionResult> ResetPassword(int id, [FromBody] ResetPasswordRequest body)
            {
                if (string.IsNullOrWhiteSpace(body.NewPassword))
                    return BadRequest(new { message = "New password is required" });
                if (body.NewPassword.Length < 6)
                    return BadRequest(new { message = "Password must be at least 6 characters" });

                var user = await _context.Users.FindAsync(id);
                if (user is null) return NotFound(new { message = "User not found" });

                user.Password = _passwordHasher.HashPassword(user, body.NewPassword);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Password reset successfully" });
            }

            // -- PATCH /api/users/:id/status --------------------------
            // Admin only — toggle isActive.
            [HttpPatch("{id}/status")]
            public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateStatusRequest body)
            {
                if (body.IsActive is null)
                    return BadRequest(new { message = "isActive is required" });

                var user = await _context.Users.FindAsync(id);
                if (user is null) return NotFound(new { message = "User not found" });

                user.IsActive = body.IsActive.Value ? 1 : 0;
                await _context.SaveChangesAsync();

                return Ok(ToResponse(user));
            }

            // ── DELETE /api/users/:id ─────────────────────────────────────
            // Admin only — permanently delete a user.
            // Blocked if user has any assigned tasks.
            [HttpDelete("{id}")]
            public async Task<IActionResult> DeleteUser(int id)
            {
                var hasTasks = await _context.Main_Tasks // Check if the Task has ANY assigned ID
                    .AnyAsync(t => t.AssigneeId == id || t.QaAssigneeId == id);

                if (hasTasks)
                    return BadRequest(new { message = "Cannot delete: this user has assigned tasks. Reassign or remove their tasks first." });

                var user = await _context.Users.FindAsync(id);
                if (user is null) return NotFound(new { message = "User not found" });

                _context.Users.Remove(user);
                await _context.SaveChangesAsync();

                return Ok(new { message = "User deleted" });
            }

        }
    }
