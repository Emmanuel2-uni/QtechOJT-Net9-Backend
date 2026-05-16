namespace QtechOJT_Net9.DTO.SubTask
{
    public class EditSubTaskDto // Classes are mutable, ideally we use these for patch/put methods especially when we have to modify an object in transit
    {
        public required string Title { get; set; } = string.Empty;
        
    };
}
