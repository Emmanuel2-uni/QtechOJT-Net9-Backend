namespace QtechOJT_Net9.DTO.Status
{
    public class StatusRequestDto
    {
        public required string Label { get; set; }
        public string? Color { get; set; }
        public int SortOrder { get; set; } = 0;
        public int IsDefault { get; set; } = 0;
        public int IsFinal { get; set; } = 0;
    }
}
