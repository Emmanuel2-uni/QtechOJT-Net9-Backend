using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace QtechOJT_Net9.Models
{
    //   CREATE TABLE `phases` (
    //  `id` int (11) NOT NULL,
    //  `label` varchar(100) NOT NULL,
    //  `sortOrder` int (11) NOT NULL DEFAULT 0,
    //  `isDefault` tinyint(1) NOT NULL DEFAULT 0,
    //  `isFinal` tinyint(1) NOT NULL DEFAULT 0,
    //  `grouping` enum('dev','qa') NOT NULL DEFAULT 'dev'
    //) ENGINE=InnoDB DEFAULT CHARSET=latin1 COLLATE = latin1_swedish_ci;

    // Groupings; Dev = 1, QA = 2
    // Microsoft Server SQL does NOT support enums, so I use classic int flags for this column

    [Index(nameof(Label), IsUnique = true)]
    public class Phase
    {
        public int Id { get; set; }

        [MaxLength(100)]
        public required string Label { get; set; }


        public required int SortOrder { get; set; } = 0;
        public required int IsDefault { get; set; } = 0;
        public required int IsFinal { get; set; } = 0;
        public required string Grouping { get; set; } = "dev"; // Groupings, Server SQL does not support enums, so we manually just set the default string
        public ICollection<Main_Task> Main_Tasks { get; set; } = []; // Phases contain main_tasks, 1:M relationship

        //public int? StatusId; // FK to Status table
        //public Status? DefaultStatus { get; set; } // Default status request, 
    }
}
