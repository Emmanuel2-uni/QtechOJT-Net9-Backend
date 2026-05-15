using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QtechOJT_Net9.Models
{

    [Index(nameof(Title), IsUnique = true)]
    public class Project
    {
        public int Id { get; set; }

        [MaxLength(255)]
        public required string Title { get; set; }
        public string? Description { get; set; }
        public required string ClientName { get; set; }

        public required string Status { get; set; } = "ongoing";

        //[Column("pmId")]
        public int? PmId { get; set; }
        public User? PM { get; set; } = null!;

        public required DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime TargetEndDate { get; set; }

        public ICollection<Main_Task> Main_Tasks { get; set; } = [];

    }
}
