namespace QtechOJT_Net9.DTO.SubTask
{
    public record GetSubTaskDto
    (
        int Id,
        int? CreatorId,
        string Title,
        int IsDone,
        int Main_TaskId
    );
    
}
