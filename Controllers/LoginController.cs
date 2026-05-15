using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QtechOJT_Net9.Database;
using QtechOJT_Net9.Models;
using QtechOJT_Net9.DTO.Auth;


namespace QtechOJT_Net9.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController(KanbanDbContext context, IPasswordHasher<User> passwordHasher) : ControllerBase
    {
        private readonly KanbanDbContext _context = context;
        private readonly IPasswordHasher<User> _passwordHasher = passwordHasher;

        // Simeple Login Route
        // Login/Post request
        [HttpPost]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto req)
        {
            if (string.IsNullOrWhiteSpace(req.Username) || string.IsNullOrWhiteSpace(req.Password))
                return BadRequest(new { message = "Username and password are required" });

            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Username == req.Username);

            if (user is null)
                return NotFound(new { message = "User not found" });

            var result = _passwordHasher.VerifyHashedPassword(user, user.Password, req.Password);

            if (result == PasswordVerificationResult.Failed)
                return Unauthorized(new { message = "Invalid credentials" });

            // A guard to immediately block the access of the user if user is disabled.
            if (user.IsActive == 0)
                return Unauthorized(new { message = "User has been disabled. Please inquire an Admin." });

            return Ok(new
                        {
                            message = "Login successful",
                            user = new
                            {
                                user.Id,
                                user.Name,
                                user.Username,
                                user.Role,
                                user.IsActive
                            }
                        }
                    );
        }

    }
}
