using QtechOJT_Net9.DTO.Phase;
using QtechOJT_Net9.DTO.Severity;
using QtechOJT_Net9.DTO.Status;
using QtechOJT_Net9.DTO.User;

namespace QtechOJT_Net9.DTO.MainTask
{
    //  const {
    //  title, description, projectId,
    //  phaseId, statusId, severityId,
    //  assigneeId, qaAssigneeId, targetDate,
    //  } = req.body;
     public class PostMain_TaskDto
        {
        public string Title { get; set; }
        public string? Description { get; set; }
        public int ProjectId { get; set; }

        public int PhaseId { get; set; }
        public int StatusId { get; set; }
        public int? SeverityId { get; set; } = 0;

        public int? AssigneeId { get; set; }
        public int? QaAssigneeId { get; set; }

        public DateTime? StartDate { get; set; } = DateTime.Now;
        public DateTime? TargetDate { get; set; } = DateTime.Now;

    }
}
