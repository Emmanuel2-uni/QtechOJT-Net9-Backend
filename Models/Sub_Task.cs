namespace QtechOJT_Net9.Models
{
    public class Sub_Task
    {
        public int Id { get; set; }
        public required string Title { get; set; }
        public required int IsDone { get; set; }

        // This is the Foreign Key setup using Code-First migration,
        //  distinguished through {ObjectName}Id, this will tell EFCore to use this as the FK
        public required int Main_TaskId { get; set; } // FK to Main_Task
        public Main_Task Main_Task { get; set; } = null!;

        // FK to Creator
        public int? CreatorId { get; set; } // Again, nullable so that database doesn't explode due to existing entries without CreatorId
        public User? Creator { get; set; }

        public ICollection<Sub_Task_Comment> Sub_Task_Comments { get; set; } // change this variable name when needed, if FE fetching is problematic

    }
}
