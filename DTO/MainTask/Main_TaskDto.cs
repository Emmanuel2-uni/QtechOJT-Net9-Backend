using QtechOJT_Net9.DTO.Phase;
using QtechOJT_Net9.DTO.Severity;
using QtechOJT_Net9.DTO.Status;
using QtechOJT_Net9.DTO.SubTask;
using QtechOJT_Net9.DTO.User;
using QtechOJT_Net9.Models;
using System.Threading.Tasks;

namespace QtechOJT_Net9.DTO.MainTask
{
        //  `SELECT
        //     t.id, t.projectId, t.title, t.description, t.progress,
        //     t.targetDate, t.actualEndDate, t.createdAt, t.updatedAt,
        //     t.phaseId,      p.label AS phaseLabel,
        //     p.isFinal AS phaseIsFinal,
        //     p.isDefault AS phaseIsDefault,
        //     p.grouping AS phaseGrouping,
        //     t.statusId,     s.label AS statusLabel,
        //     t.severityId,   sv.label AS severityLabel,
        //     t.assigneeId,   u.name AS assigneeName,   u.username AS assigneeUsername,
        //     t.qaAssigneeId, qa.name AS qaAssigneeName, qa.username AS qaAssigneeUsername
        //   FROM tasks t
        //   LEFT JOIN phases p  ON t.phaseId      = p.id
        //   LEFT JOIN statuses   s ON t.statusId     = s.id
        //   LEFT JOIN severities sv ON t.severityId   = sv.id
        //   LEFT JOIN users u  ON t.assigneeId   = u.id
        //   LEFT JOIN users qa ON t.qaAssigneeId = qa.id
        //   WHERE t.id = ?`,
        //  [id]
        //);

    // These is a POSITIONAL Record, it does not { get; init; } so this functions as an object constructor?
    public record Main_TaskDto(
        int Id, 
        int ProjectId,
        string Title, 
        string Description, 
        int Progress,

        DateTime UpdatedAt, 
        DateTime CreatedAt, 
        DateTime? ActualEndDate, 
        DateTime TargetDate,

        int? AssigneeId,
        string? AssigneeName,
        string? AssigneeUsername,

        int? QaAssigneeId, 
        string? QaAssigneeName,
        string? QaAssigneeUsername,

        int PhaseId,
        int PhaseIsFinal,
        int PhaseIsDefault,
        string PhaseGrouping,

        int SeverityId,
        string SeverityLabel,

        int StatusId,
        string StatusLabel,

        //StatusDto Status,
        //SeverityDto Severity,
        //UserDto? Dev,
        //UserDto? QA,
        //PhaseDto Phase,
        int? Variance,
        List<GetSubTaskDto> SubTasks

        );
}
