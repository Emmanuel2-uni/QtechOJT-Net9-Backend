namespace QtechOJT_Net9.DTO.Project
{
    public class ProjectResponseDto
    {
        public int Id { get; init; }
        public string Title { get; init; } = string.Empty;
        public string? Description { get; init; }
        public string? ClientName { get; init; }
        public DateTime? TargetEndDate { get; init; }
        public string? Status { get; init; }
        public DateTime CreatedAt { get; init; }
        public int? PmId { get; init; }
        public string? PmName { get; init; }
        public string? PmUsername { get; init; }
        public int TaskCount { get; init; }
    }
}
