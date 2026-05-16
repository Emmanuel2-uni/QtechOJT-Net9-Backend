namespace QtechOJT_Net9.DTO.MainTask
{
    public class UpdateMain_TaskDto
    {
        public string Title { get; set; }
        public string Description { get; set; }

        public DateTime? StartDate { get; set; }
        public DateTime? TargetDate { get; set; }

        public int? StatusId { get; set; }
        public int? SeverityId { get; set; }
        public int? AssigneeId { get; set; }
        public int? QaAssigneeId { get; set; }


    }
}
