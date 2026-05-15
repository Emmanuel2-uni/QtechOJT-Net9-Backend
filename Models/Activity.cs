using System;
using static System.Net.Mime.MediaTypeNames;

namespace QtechOJT_Net9.Models
{
        // From DB schema export log
        //  CREATE TABLE `activity_logs` (
        //  `id` int(11) NOT NULL,
        //  `taskId` int (11) NOT NULL,
        //  `userId` int (11) DEFAULT NULL,
        //  `action` text NOT NULL,
        //  `createdAt` datetime NOT NULL DEFAULT current_timestamp()
        //) ENGINE=InnoDB DEFAULT CHARSET=latin1 COLLATE = latin1_swedish_ci;

    public class Activity
    {
        public int Id { get; set; }

        // FK to Main_Task
        public int? Main_TaskId { get; set; }
        public Main_Task Main_Task { get; set; } = null!;

        // FK to User
        public int? UserId { get; set; } = null;
        public User User { get; set; } = null!;


        public required string ActionDone { get; set; }
        public required DateTime CreatedAt { get; set; } // set it to default

    }
}
