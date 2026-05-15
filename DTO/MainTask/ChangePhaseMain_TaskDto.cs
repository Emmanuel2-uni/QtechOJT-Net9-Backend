using QtechOJT_Net9.Models;

namespace QtechOJT_Net9.DTO.MainTask
{
    public record ChangePhaseMain_TaskDto(

        //  const { id }                     = req.params;
        //  const { phaseId, actualEndDate
        //         } = req.body;
        //  const userId = req.headers["x-user-id"] ? Number(req.headers["x-user-id"]) : null;

        int PhaseId,
        DateTime? ActualEndDate
        
        );
}
