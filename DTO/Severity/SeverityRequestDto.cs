namespace QtechOJT_Net9.DTO.Severity
{
    public record SeverityRequestDto(
        string Label, 
        string? Color, 
        int SortOrder = 0
        );
}
