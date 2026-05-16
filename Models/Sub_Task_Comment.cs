namespace QtechOJT_Net9.Models
{
    public class Sub_Task_Comment
    {
        public int Id { get; set; }

        // Sub_Task FK
        public int Sub_TaskId { get; set; }
        public Sub_Task Sub_Task { get; set; } = null!;

        // User/Commenter FK — nullable
        public int? UserId { get; set; } // This is already the Creator, we don't need to add a CreatorId
        public User? User { get; set; }

        public string? Comment { get; set; }
        public required DateTime CommentDate { get; set; } = DateTime.UtcNow;
    }
}
