namespace QtechOJT_Net9.DTO.MainTask
{
    public record GeneralMain_TaskByIdDto(
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

            int? QaAssigneeId,
            string? QaAssigneeName,

            int StatusId,
            string StatusLabel,

            int SeverityId,
            string SeverityLabel,
            string SeverityColor,
            int SeveritySortOrder,

            int PhaseId,
            string PhaseLabel,
            string PhaseGrouping,

            int? Variance,
            int? Mandays

        );
}
