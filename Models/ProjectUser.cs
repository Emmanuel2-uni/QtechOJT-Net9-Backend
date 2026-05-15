namespace QtechOJT_Net9.Models
{
    // This model represents the junction table for the Many-to-Many relationship between Projects and Users
    public class ProjectUser
    {
        public int Id { get; set; }

        // FK abstraction entity framework to refer to Project
        public int ProjectId { get; set; }
        public Project Project { get; set; }

        // FK abstraction entity framework to refer to User
        public int UserId { get; set; }
        public User User { get; set; }

        public string Role { get; set; }
    }
}
