namespace QtechOJT_Net9.DTO.Phase
{
    public class UpdatePhaseDto
    {
        public int? Id { get; set; }
        public string Label { get; set; } = string.Empty;
        public int SortOrder { get; set; } = 0;
        public int IsDefault { get; set; } = 0;
        public int IsFinal { get; set; } = 0;
        public string Grouping { get; set; } = "dev";

        public int? DefaultStatusId { get; set; }
    }
}
