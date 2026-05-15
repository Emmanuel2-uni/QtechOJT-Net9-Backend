namespace QtechOJT_Net9.DTO.User
{
    public record CreateUserRequest(
        string? Name, 
        string? Username, 
        string? Password, 
        string? Role
        );
}
