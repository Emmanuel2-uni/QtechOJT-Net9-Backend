namespace QtechOJT_Net9.DTO.User
{
    public record UserResponse(
        int Id, 
        string Name, 
        string Username, 
        string Role, 
        int IsActive
        );
}
