using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace QtechOJT_Net9.Models
{
    [Index(nameof(Label), IsUnique = true)]
    public class Severity
    {

        public int Id { get; set; }

        [MaxLength(255)]
        public required string Label { get; set; }
        public required int SortOrder { get; set; } = 0;
        public string? Color { get; set; } = null;
        public ICollection<Main_Task>? Main_Tasks { get; set; } = []; // FK relation from Main_Tasks
    }
}
