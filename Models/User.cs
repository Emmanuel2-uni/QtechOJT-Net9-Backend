using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace QtechOJT_Net9.Models
{
  
    [Index(nameof(Username), IsUnique = true)]
    public class User
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string Role { get; set; } = "Developer"; // Admin = 1, ProjectManager = 2, Developer = 3, QA = 4

        [MaxLength(255)]
        public required string Username { get; set; } 
        public required string Password { get; set; }
        public required int IsActive { get; set; } = 1;

        //public ICollection<Main_Task> Main_Task { get; set; } = []; // FK from MT

        public ICollection<Main_Task>? DevTasks { get; set; } = [];
        public ICollection<Main_Task>? QATasks { get; set; } = [];
        public ICollection<Project> Project { get; set; } = []; // FK from Project
        public ICollection<Activity> Activity { get; set; } = []; // FK from Activity Log


    }
}
