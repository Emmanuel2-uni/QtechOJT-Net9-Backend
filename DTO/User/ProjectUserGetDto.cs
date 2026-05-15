namespace QtechOJT_Net9.DTO.User
{
    public record ProjectUserGetDto(
        int Id,
        int ProjectId,
        int UserId,
        string Role,
        string Name
    );
}
