namespace QtechOJT_Net9.DTO.Project
{
    public class ProjectRequestDto
    {
        public string Title { get; set; }
        public string? Description { get; set; }
        public int? PmId { get; set; }
        public string? ClientName { get; set; }
        public DateTime? TargetEndDate { get; set; } 
        public string? Status { get; set; }
        public List<int>? Developers { get; set; } = [];
        public List<int>? Qas { get; set; } = [];
    };
}
