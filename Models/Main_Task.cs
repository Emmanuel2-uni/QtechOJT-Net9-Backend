using System.ComponentModel.DataAnnotations.Schema;

namespace QtechOJT_Net9.Models
{
    public class Main_Task
    {


        // Variables for basics of the Task
        // Naming convention for C# in general is PascalCase
        // EFCore serializes DB communications to camelCase automatically
        public int Id { get; set; } // EFCore sets this to Auto-Increment or "Identity" (MS SQL definition) and as the Primary Key automatically

        // FK to Project
        public int ProjectId { get; set; } // EFCore will know that these is a Foreign Key to the Projects table, syntax {Model}Id
        public Project Project { get; set; } = null!; // Entity initializer so that EFCore knows what to do with ProjectId

        public required string Title { get; set; }
        public string? Description { get; set; }
        public required int Progress { get; set; } = 0;
        


        // Variable for subtasks
        // ICollection declaration below tells us that the Main_Tasks will have a collection of Sub_Tasks
        // Important note: I got an error when communicating to the Frontend where the subtask array is getting called using
        //   "subtasks" not "SubTasks" or "Sub_Tasks". Naming is very important so debugging by reading Frontend code is a necessary skill too, I think ~_~
        public ICollection<Sub_Task> Subtasks { get; set; } = []; // FK from Sub_Task


        //Variables for determining the date AND the variance once finished
        public required DateTime CreatedAt { get; set; }

        public DateTime? StartDate { get; set; }
        public required DateTime TargetDate { get; set; }
        public DateTime? ActualEndDate { get; set; }
        public int? Variance { get; set; } = null;
        public int? Mandays { get; set; } = null;
        public required DateTime UpdatedAt { get; set; } = DateTime.Now;


        // Variables for determining which phase and project this task belongs to
        // For database joining/relational purposes



        // FK to User
        // Note to self, prior migrations failed because setting FKs to REQUIRED/non-nullables tells EFCore to duplicate it
        // Remove all required from FKs since lists should be empty

        //public int UserId { get; set; }
        //public User User { get; set; } = null!;

        // Nav properties for FK to UserId
        public int? AssigneeId { get; set; }
        public User? Assignee { get; set; }

        public int? QaAssigneeId { get; set; }
        public User? QaAssignee { get; set; }

        //public int? CreatorId { get; set; }
        //public User? Creator {  get; set; }


        // FK to Phase
        public int PhaseId { get; set; }
        public Phase Phase { get; set; } = null!;

        // FK to Status
        public int StatusId { get; set; }
        public Status Status { get; set; } = null!;

        // FK to Severity
        public int? SeverityId { get; set; }
        public Severity? Severity { get; set; }

        public ICollection<Activity> Activity_Log { get; set; } = []; // FK from Activity Log
    }
}
