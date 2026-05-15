namespace QtechOJT_Net9.DTO.MainTask
{

            //    router.get("/", async(req, res) => {
            //  const { projectId, statusId, phaseId
            //} = req.query;
            //let query = `
            //    SELECT 
            //      t.*,
            //    st.label  AS statusLabel,
            //    st.color  AS statusColor,
            //    s.label as severityLabel,
            //    s.color as severityColor,
            //    s.sortOrder as severitySortOrder,
            //    st.label as statusLabel,
            //    p.label as phaseLabel,
            //    p.grouping as phaseGrouping,
            //    assignee.name as assigneeName,
            //    qaAssignee.name as qaAssigneeName
            //    FROM tasks t
            //    LEFT JOIN severities s ON t.severityId = s.id
            //    LEFT JOIN statuses st ON t.statusId = st.id
            //    LEFT JOIN phases p ON t.phaseId = p.id
            //    LEFT JOIN users assignee ON t.assigneeId = assignee.id
            //    LEFT JOIN users qaAssignee ON t.qaAssigneeId = qaAssignee.id
            //    WHERE 1=1
            //  `;
        public record GeneralMain_TaskDto (
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
            string StatusColor, 

            int SeverityId,
            string SeverityLabel,
            string SeverityColor,
            int SeveritySortOrder,

            int PhaseId,
            string PhaseLabel,
            string PhaseGrouping



        );
}
