using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace QtechOJT_Net9.Models
{
    //    CREATE TABLE `assessments` (
    //  `id` int (11) NOT NULL,
    //  `label` varchar(100) NOT NULL,
    //  `sortOrder` int (11) NOT NULL DEFAULT 0
    //) ENGINE=InnoDB DEFAULT CHARSET=latin1 COLLATE = latin1_swedish_ci;

    [Index(nameof(Label), IsUnique = true)]
    public class Assessment
    {
        public int Id { get; set; }

        [MaxLength(255)]
        public required string Label { get; set; }
        public required int SortOrder { get; set; } = 0;
    }
}
