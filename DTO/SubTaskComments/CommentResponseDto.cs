namespace QtechOJT_Net9.DTO.SubTaskComments
{
    public record CommentResponseDto(
        int Id,
        int Sub_TaskId,
        int? UserId,
        string? Comment,
        DateTime CommentDate,
        string? CommenterName,
        string? CommenterUsername
    );
}
