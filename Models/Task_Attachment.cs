namespace QtechOJT_Net9.Models
{
    public class Task_Attachment
    {
        public int Id { get; set; }

        // FK to Main_Task
        public required int Main_TaskId { get; set; }
        public Main_Task Main_Task { get; set; } = null!;

        public required string Filename { get; set; }
        public required string OriginalName { get; set; }
        public required string Mimetype { get; set; }

        public required int Size { get; set; }

        public DateTime UploadedAt { get; set; } = DateTime.Now;

    }
}
