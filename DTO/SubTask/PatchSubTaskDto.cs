namespace QtechOJT_Net9.DTO.SubTask
{
    public class PatchSubTaskDto // Classes are mutable, ideally we use these for patch/put methods especially when we have to modify an object in transit
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public bool IsDone { get; set; }
    };
}
