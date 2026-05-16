namespace QtechOJT_Net9.DTO.SubTask
{
    public record SubTaskItemDto(
        int Id, 
        int CreatorId,
        string Title, 
        bool IsDone   );
}
