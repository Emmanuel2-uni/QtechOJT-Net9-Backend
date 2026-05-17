using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace QtechOJT_Net9.Models
{
    [Index(nameof(Label), IsUnique = true)]
    public class Status
    {
        public int Id { get; set; }

        [MaxLength(255)]
        public required string Label { get; set; }

        public required int SortOrder { get; set; } = 0;
        public required int IsDefault { get; set; } = 0;
        public required int IsFinal { get; set; } = 0;
        public required string Color { get; set; } = "#6b7280";
        public ICollection<Main_Task> Main_Tasks { get; set; } = []; // FK relation from Main_Tasks
        public ICollection<Phase> Phases { get; set; } = []; // FK relation from Main_Tasks


    }
}
