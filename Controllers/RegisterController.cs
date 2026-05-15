using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QtechOJT_Net9.Database;
using QtechOJT_Net9.Models;
using QtechOJT_Net9.DTO.Auth;

namespace QtechOJT_Net9.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RegisterController (KanbanDbContext context, IPasswordHasher<User> passwordHasher): ControllerBase
    {

        private readonly KanbanDbContext _context = context;
        private readonly IPasswordHasher<User> _passwordHasher = passwordHasher;

        // Just a simple POST method for creating users.
        // Ideally, only the admin can do this.
        [HttpPost]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDto req)
        {
            if (string.IsNullOrWhiteSpace(req.Username) || string.IsNullOrWhiteSpace(req.Password))
                return BadRequest(new { message = "Username and password are required" });

            if (req.Password.Length < 8)
                return BadRequest(new { message = "Password must be at least 8 characters long." });

            bool userExists = await _context.Users.AnyAsync(u => u.Username == req.Username);
            if (userExists)
                return BadRequest(new { message = "User already exists" });

            var user = new User
            {
                Name = string.IsNullOrWhiteSpace(req.Name) ? req.Username : req.Name,
                Username = req.Username,
                Password = string.Empty, // temporary, hashed below
                Role = "Developer", // A default role, if nothing is selected then default will be Developer
                IsActive = 1
            };

            user.Password = _passwordHasher.HashPassword(user, req.Password);

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return StatusCode(201, new { message = "User Created" });
        }
    }

}
