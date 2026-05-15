using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using QtechOJT_Net9.Models;
using System.Collections.Generic;
using System.Xml;

namespace QtechOJT_Net9.Database

{
    public class KanbanDbContext(DbContextOptions<KanbanDbContext> options) : DbContext(options)
    {
        // Code-First Migration practice
        // I encountered an error here where I was apparently setting a type, where the name is used as a namespace
        // Solution here: I renamed the folder Main_Task to MainTask to separate the DTO namespace from the type/class
        // A better practice here is probably to append DTO or Model [Main_TaskDTO / Main_TaskModel] to each folder name so that the namespace is clear and fixable

        // Update-Database -Migration 0 to completely reset database and stuff
        // remove-migration one by one



        // Tell EFCore to make these Tables based on the Model set in the Entity/Object syntax
        /// syntax == DbSet<Entity> [NameOfTable] => Set<Entity>(); 
        public DbSet<Main_Task> Main_Tasks => Set<Main_Task>();
        public DbSet<Sub_Task> Sub_Tasks => Set<Sub_Task>();
        public DbSet<User> Users => Set<User>();
        public DbSet<Phase> Phases => Set<Phase>();
        public DbSet<Severity> Severities => Set<Severity>();
        public DbSet<Status> Statuses => Set<Status>();
        public DbSet<Activity> Activity_Log => Set<Activity>();
        public DbSet<Assessment> Assessments => Set<Assessment>();
        public DbSet<Project> Projects => Set<Project>();
        public DbSet<Holiday> Holidays => Set<Holiday>();
        public DbSet<ProjectUser> Project_Users => Set<ProjectUser>();
        public DbSet<Sub_Task_Comment> Sub_Task_Comments => Set<Sub_Task_Comment>();
        public DbSet<Task_Attachment> Task_Attachments => Set<Task_Attachment>();



        //// ======================================================
        //// ========== Options section ===========================
        // THESE ARE ALL STILL READ AS CODE, MEANING IT IS READ UP TO DOWN.
        //   Reminder to self: When setting properties, seeding should be done AFTER all entity properties have been set.
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            // Unique Key Entity options
            modelBuilder.Entity<Assessment>()
                    .HasIndex(a => a.Label)
                    .IsUnique();

                modelBuilder.Entity<Severity>()
                    .HasIndex(s => s.Label)
                    .IsUnique();

                modelBuilder.Entity<Phase>()
                    .HasIndex(p => p.Label)
                    .IsUnique();

                modelBuilder.Entity<Project>()
                    .HasIndex(p => p.Title)
                    .IsUnique();

                modelBuilder.Entity<Status>()
                    .HasIndex(s => s.Label)
                    .IsUnique();

                modelBuilder.Entity<User>()
                    .HasIndex(u => u.Username)
                    .IsUnique();


            // 1 to Many == Activity : User
            // 1 to Many == Task : User
            modelBuilder.Entity<Activity>()
                    .HasOne(u=>u.User)
                    .WithMany(a=>a.Activity)
                    .HasForeignKey(u=>u.UserId)
                    .IsRequired(false)
                    .OnDelete(DeleteBehavior.SetNull); // Do not delete the User if the User entries are deleted

            // Commented out until a better solution for logging is made
            // I dont think the task entries should be deleted in the activity log if the task itself is also deleted
            modelBuilder.Entity<Activity>()
                    .HasOne(t => t.Main_Task)
                    .WithMany(a => a.Activity_Log)
                    .HasForeignKey(u => u.Main_TaskId)
                    .IsRequired(false)
                    .OnDelete(DeleteBehavior.SetNull);

            // ---- Main_Tasks --------------------------------------
            // Model binding technique to set a backup plan for the backend
            // redundant but a failsafe
            modelBuilder.Entity<Main_Task>()
                .Property(m => m.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .ValueGeneratedOnAdd(); //Unmodifiable

            modelBuilder.Entity<Main_Task>()
                .Property(m => m.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .ValueGeneratedOnAdd(); //Unmodifiable

            modelBuilder.Entity<Main_Task>()
                .Property(m => m.Progress)
                .HasDefaultValue(0);

            modelBuilder.Entity<Main_Task>()
                .Property(m => m.StatusId)
                .HasDefaultValue(1);

            modelBuilder.Entity<Main_Task>(entity =>
            {
                // Developer FK
                entity.HasOne(t => t.Assignee) // Has one user (declared in the main_task model as a navigation property)
                      .WithMany(u => u.DevTasks) // for many tasks (declared in users model because a USER can have many tasks) POINTS TO USER TABLE
                      .HasForeignKey(t => t.AssigneeId) // Find the Id to the Assignee, since we are pointing to the USER TABLE, the Id here is the Id of the user.
                      .IsRequired(false) // for future-proofing the code as nullable
                      .OnDelete(DeleteBehavior.Restrict); // If I delete a task, it should not delete the Dev lmao

                // QA FK
                entity.HasOne(t => t.QaAssignee)
                      .WithMany(u => u.QATasks)
                      .HasForeignKey(t => t.QaAssigneeId)
                      .IsRequired(false)
                      .OnDelete(DeleteBehavior.Restrict); // If I delete a task, it should not delete the QA

                entity.HasOne(t => t.Severity)
                      .WithMany(s => s.Main_Tasks)
                      .HasForeignKey(t => t.SeverityId)
                      .IsRequired(false)
                      .OnDelete (DeleteBehavior.Restrict); // If I delete a task, it should not delete the severity
            });

            modelBuilder.Entity<Project>(entity =>
            {
                // PM FK
                entity.HasOne(u => u.PM)
                      .WithMany(p => p.Project)
                      .HasForeignKey(u => u.PmId)
                      .IsRequired(false)
                      .OnDelete(DeleteBehavior.Restrict); // If I delete a project, it should not delete the PM
            });

            // ----- Statuses --------------------------
            // Multiple lambda in one fluent api constructor
            modelBuilder.Entity<Status>(entity =>
            {
                entity.Property(s => s.SortOrder)
                        .HasDefaultValue(0);
                entity.Property(d => d.IsDefault)
                        .HasDefaultValue(0);
                entity.Property(f => f.IsFinal)
                        .HasDefaultValue(0);
            });

            modelBuilder.Entity<Phase>()
                .Property(p => p.Label)
                .HasColumnType("nvarchar(100)"); // For Unicode support

            // ---- User settings -----------------
            //modelBuilder.Entity<User>(entity =>
            //    {
            //        entity.Property(u => u.Role)
            //            .HasDefaultValue(0)
            //            .ValueGeneratedOnAdd();
            //    }
            //);

            // ----- Phases --------------------------
            // Phase default settings
            modelBuilder.Entity<Phase>(entity =>
            {
                entity.Property(s => s.SortOrder)
                    .HasDefaultValue(0);
                entity.Property(s => s.IsDefault)
                    .HasDefaultValue(0);
                entity.Property(s => s.Grouping)
                    .HasDefaultValue(1);
            });

            // ---- Assessments --------------------
            // Property Settings
            modelBuilder.Entity<Phase>(entity =>
            {
                entity.Property(p => p.SortOrder)
                    .HasDefaultValue(0);
            });

            // -- Sub_Task Comments -------
            // Reinforcing an non-required FK relation
            modelBuilder.Entity<Sub_Task_Comment>()
                .HasOne(c => c.User)
                .WithMany()
                .HasForeignKey(c => c.UserId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);



            //// ======================================================
            //// ========== Seeding section ===========================
            // -- Users --------------------------------------
            // User Seed

            // REMOVE 
            modelBuilder.Entity<User>()
                .HasData(
                    new User { Id = 1, Name = "Admin User", Username = "admin", Password = "AQAAAAIAAYagAAAAEI88UJCZYAeB6zVYAb3w/j29tn4IEBWuoYQGSF2K6AzHYttOvYDsl+fc31yGv7B4tQ==", Role = "Admin", IsActive = 1 }
                    //new User { Id = 2, Name = "Carlo Reyes", Username = "carlo", Password = "$2b$10$YuJ6Or8HNFOKs3w3elGep.4JPfegjkjPzEogdb86q.fz.z7hjilOS", Role = "Developer", IsActive = 1 },
                    //new User { Id = 3, Name = "Ana Santos", Username = "ana", Password = "$2b$10$YuJ6Or8HNFOKs3w3elGep.4JPfegjkjPzEogdb86q.fz.z7hjilOS", Role = "Developer", IsActive = 1 },
                    //new User { Id = 4, Name = "Dana Cruz", Username = "dana", Password = "$2b$10$YuJ6Or8HNFOKs3w3elGep.4JPfegjkjPzEogdb86q.fz.z7hjilOS", Role = "QA", IsActive = 1 },
                    //new User { Id = 5, Name = "Ben Torres", Username = "ben", Password = "$2b$10$YuJ6Or8HNFOKs3w3elGep.4JPfegjkjPzEogdb86q.fz.z7hjilOS", Role = "Developer", IsActive = 1 },
                    //new User { Id = 6, Name = "Maria Lopez", Username = "maria", Password = "$2b$10$YuJ6Or8HNFOKs3w3elGep.4JPfegjkjPzEogdb86q.fz.z7hjilOS", Role = "ProjectManager", IsActive = 1 },
                    //new User { Id = 7, Name = "Mark Yyu", Username = "markyyu", Password = "$2b$10$bf2rKJo.iivVeXrMRo2gKu1SHI71ZL.Hf4n1M1xRnqPR6FcXdpjoO", Role = "ProjectManager", IsActive = 1 }
            );

            // These are exports from MySQL with a BCrypt hashing JS route
            // Note to self: Reset these seeds later!!!!!! (Or not)

            // REMOVE OR COMMENT
            //modelBuilder.Entity<Main_Task>()
            //    .HasData(
            //        new Main_Task
            //        {
            //            Id = 1,
            //            ProjectId = 1,
            //            Title = "Design login page",
            //            Description = "Create Figma mockup and implement HTML/CSS.",
            //            StatusId = 1,
            //            PhaseId = 3,
            //            SeverityId = 2,
            //            AssigneeId = 3,
            //            QaAssigneeId = null,
            //            TargetDate = new DateTime(2025, 4, 10),
            //            ActualEndDate = null,
            //            Progress = 0,
            //            CreatedAt = new DateTime(2026, 4, 18, 14, 45, 8),
            //            UpdatedAt = new DateTime(2026, 4, 19, 11, 53, 14)
            //        },
            //        new Main_Task
            //        {
            //            Id = 2,
            //            ProjectId = 1,
            //            Title = "Write API docs",
            //            Description = "Document all Express routes via Postman.",
            //            StatusId = 1,
            //            PhaseId = 3,
            //            SeverityId = 4,
            //            AssigneeId = 5,
            //            QaAssigneeId = 4,
            //            TargetDate = new DateTime(2025, 4, 20),
            //            ActualEndDate = null,
            //            Progress = 0,
            //            CreatedAt = new DateTime(2026, 4, 18, 14, 45, 8),
            //            UpdatedAt = new DateTime(2026, 4, 19, 11, 53, 14)
            //        },
            //        new Main_Task
            //        {
            //            Id = 3,
            //            ProjectId = 1,
            //            Title = "Build dashboard UI",
            //            Description = "Implement analytics dashboard with charts.",
            //            StatusId = 2,
            //            PhaseId = 3,
            //            SeverityId = 2,
            //            AssigneeId = 2,
            //            QaAssigneeId = 4,
            //            TargetDate = new DateTime(2025, 4, 15),
            //            ActualEndDate = null,
            //            Progress = 55,
            //            CreatedAt = new DateTime(2026, 4, 18, 14, 45, 8),
            //            UpdatedAt = new DateTime(2026, 4, 22, 20, 35, 49)
            //        },
            //        new Main_Task
            //        {
            //            Id = 4,
            //            ProjectId = 1,
            //            Title = "Auth endpoints",
            //            Description = "Express JWT auth with bcrypt hashing.",
            //            StatusId = 6,
            //            PhaseId = 7,
            //            SeverityId = 1,
            //            AssigneeId = 4,
            //            QaAssigneeId = 6,
            //            TargetDate = new DateTime(2025, 4, 10),
            //            ActualEndDate = new DateTime(2026, 4, 18),
            //            Progress = 100,
            //            CreatedAt = new DateTime(2026, 4, 18, 14, 45, 8),
            //            UpdatedAt = new DateTime(2026, 4, 19, 12, 47, 47)
            //        },
            //        new Main_Task
            //        {
            //            Id = 5,
            //            ProjectId = 1,
            //            Title = "Project repo setup",
            //            Description = "Initialise GitHub repo and branch rules.",
            //            StatusId = 8,
            //            PhaseId = 7,
            //            SeverityId = 3,
            //            AssigneeId = 2,
            //            QaAssigneeId = 4,
            //            TargetDate = new DateTime(2025, 4, 1),
            //            ActualEndDate = null,
            //            Progress = 100,
            //            CreatedAt = new DateTime(2026, 4, 18, 14, 45, 8),
            //            UpdatedAt = new DateTime(2026, 4, 19, 12, 47, 49)
            //        },
            //        new Main_Task
            //        {
            //            Id = 6,
            //            ProjectId = 1,
            //            Title = "Kanban Frontend",
            //            Description = "test data no. 1",
            //            StatusId = 1,
            //            PhaseId = 3,
            //            SeverityId = 2,
            //            AssigneeId = null,
            //            QaAssigneeId = 4,
            //            TargetDate = new DateTime(2026, 4, 22),
            //            ActualEndDate = null,
            //            Progress = 0,
            //            CreatedAt = new DateTime(2026, 4, 19, 11, 29, 17),
            //            UpdatedAt = new DateTime(2026, 4, 19, 12, 47, 1)
            //        },
            //        new Main_Task
            //        {
            //            Id = 7,
            //            ProjectId = 2,
            //            Title = "Backshot ugh",
            //            Description = "mwehehe",
            //            StatusId = 1,
            //            PhaseId = 2,
            //            SeverityId = 2,
            //            AssigneeId = 5,
            //            QaAssigneeId = 2,
            //            TargetDate = new DateTime(2026, 4, 29),
            //            ActualEndDate = null,
            //            Progress = 0,
            //            CreatedAt = new DateTime(2026, 4, 22, 20, 32, 28),
            //            UpdatedAt = new DateTime(2026, 4, 22, 20, 34, 48)
            //        },
            //        new Main_Task
            //        {
            //            Id = 8,
            //            ProjectId = 2,
            //            Title = "blow work",
            //            Description = "dipindi",
            //            StatusId = 1,
            //            PhaseId = 2,
            //            SeverityId = 3,
            //            AssigneeId = 5,
            //            QaAssigneeId = 1,
            //            TargetDate = new DateTime(2026, 4, 23),
            //            ActualEndDate = null,
            //            Progress = 0,
            //            CreatedAt = new DateTime(2026, 4, 22, 20, 34, 42),
            //            UpdatedAt = new DateTime(2026, 4, 22, 20, 35, 44)
            //        }
            //    );
          

            ////  Sub_Task seed
            //modelBuilder.Entity<Sub_Task>()
            //    .HasData(
            //        new Sub_Task { Id = 12, Main_TaskId = 7, Title = "subtask 1", IsDone = 0 },
            //        new Sub_Task { Id = 22, Main_TaskId = 8, Title = "1", IsDone = 0 },
            //        new Sub_Task { Id = 23, Main_TaskId = 8, Title = "2", IsDone = 0 }
            //    );

            //// Project seed
            //modelBuilder.Entity<Project>()
            //    .HasData(
            //        new Project
            //        {
            //            Id = 1,
            //            Title = "QTask Development",
            //            Description = "Default project for existing tasks",
            //            ClientName = "Sikrit",
            //            Status = "ongoing",
            //            PmId = 7,
            //            TargetEndDate = new DateTime(2026, 4, 28),
            //            CreatedDate = new DateTime(2026, 4, 19, 11, 53, 14)
            //        },
            //        new Project
            //        {
            //            Id = 2,
            //            Title = "Test Project 1",
            //            Description = "test data to simulate the separation of workloads",
            //            ClientName = "Atho pooo",
            //            Status = "completed",
            //            PmId = 6,
            //            TargetEndDate = new DateTime(2026, 4, 20),
            //            CreatedDate = new DateTime(2026, 4, 19, 12, 10, 9)
            //        },
            //        new Project
            //        {
            //            Id = 3,
            //            Title = "prajekk",
            //            Description = "brip diskripsyun",
            //            ClientName = "canzon qt",
            //            Status = "ongoing",
            //            PmId = 6,
            //            TargetEndDate = new DateTime(2026, 5, 22),
            //            CreatedDate = new DateTime(2026, 4, 24, 15, 36, 3)
            //        },
            //        new Project
            //        {
            //            Id = 4,
            //            Title = "test 4",
            //            Description = null,
            //            ClientName = "testting tao",
            //            Status = "cancelled",
            //            PmId = 7,
            //            TargetEndDate = new DateTime(2026, 4, 25),
            //            CreatedDate = new DateTime(2026, 4, 24, 22, 16, 30)
            //        }
            //    );

            // Assessment seed
            modelBuilder.Entity<Assessment>()
                .HasData(
                    new Assessment { Id = 1, Label = "Existing", SortOrder = 1 },
                    new Assessment { Id = 2, Label = "Development / Customization", SortOrder = 2 },
                    new Assessment { Id = 3, Label = "Enhancement", SortOrder = 3 },
                    new Assessment { Id = 4, Label = "Not Applicable", SortOrder = 4 },
                    new Assessment { Id = 5, Label = "Out of Scope", SortOrder = 5 },
                    new Assessment { Id = 6, Label = "Defect", SortOrder = 6 }
                );

            // Phase Seed
            modelBuilder.Entity<Phase>()
                .HasData(
                    new Phase { Id = 1, Label = "Backlog (Requirements)", SortOrder = 1, IsDefault = 1, IsFinal = 0, Grouping = "dev" },
                    new Phase { Id = 2, Label = "To Do (Ready for Dev)", SortOrder = 2, IsDefault = 0, IsFinal = 0, Grouping = "dev" },
                    new Phase { Id = 3, Label = "In Progress", SortOrder = 3, IsDefault = 0, IsFinal = 0, Grouping = "dev" },
                    new Phase { Id = 4, Label = "For Review (Dev Done)", SortOrder = 4, IsDefault = 0, IsFinal = 0, Grouping = "dev" },
                    new Phase { Id = 5, Label = "Client Review - UAT", SortOrder = 5, IsDefault = 0, IsFinal = 0, Grouping = "qa" },
                    new Phase { Id = 6, Label = "QA Execution", SortOrder = 6, IsDefault = 0, IsFinal = 0, Grouping = "qa" },
                    new Phase { Id = 7, Label = "Deployed (Go-Live)", SortOrder = 7, IsDefault = 0, IsFinal = 0, Grouping = "qa" },
                    new Phase { Id = 8, Label = "Completed", SortOrder = 8, IsDefault = 0, IsFinal = 1, Grouping = "qa" }
            );

            // Severity Seed
            modelBuilder.Entity<Severity>()
                .HasData(
                    new Severity { Id = 1, Label = "1 - Critical / Showstopper", SortOrder = 1 },
                    new Severity { Id = 2, Label = "2 - High", SortOrder = 2 },
                    new Severity { Id = 3, Label = "3 - Medium", SortOrder = 3 },
                    new Severity { Id = 4, Label = "4 - Low", SortOrder = 4 },
                    new Severity { Id = 5, Label = "5 - Cosmetic Fix", SortOrder = 5 },
                    new Severity { Id = 6, Label = "Nice to Have", SortOrder = 6 }
                );

            // Status Seed
            modelBuilder.Entity<Status>()
                .HasData(
                    new Status { Id = 1, Label = "Not Started", Color = "#94a3b8", SortOrder = 1, IsDefault = 1, IsFinal = 0 },
                    new Status { Id = 2, Label = "Active", Color = "#3b82f6", SortOrder = 2, IsDefault = 0, IsFinal = 0 },
                    new Status { Id = 3, Label = "Blocked", Color = "#ef4444", SortOrder = 3, IsDefault = 0, IsFinal = 0 },
                    new Status { Id = 4, Label = "Bug Fixing", Color = "#f97316", SortOrder = 4, IsDefault = 0, IsFinal = 0 },
                    new Status { Id = 5, Label = "Clarification Needed", Color = "#f59e0b", SortOrder = 5, IsDefault = 0, IsFinal = 0 },
                    new Status { Id = 6, Label = "For Verification", Color = "#8b5cf6", SortOrder = 6, IsDefault = 0, IsFinal = 0 },
                    new Status { Id = 7, Label = "Failed", Color = "#dc2626", SortOrder = 7, IsDefault = 0, IsFinal = 0 },
                    new Status { Id = 8, Label = "Passed", Color = "#22c55e", SortOrder = 8, IsDefault = 0, IsFinal = 1 }
                );

        }
    }
}

