using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace QtechOJT_Net9.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Assessments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Label = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Assessments", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Holidays",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Holidays", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Phases",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Label = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    IsDefault = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    IsFinal = table.Column<int>(type: "int", nullable: false),
                    Grouping = table.Column<string>(type: "nvarchar(max)", nullable: false, defaultValue: "1")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Phases", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Severities",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Label = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: false),
                    Color = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Severities", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Statuses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Label = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    IsDefault = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    IsFinal = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    Color = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Statuses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Role = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Username = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Projects",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClientName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PmId = table.Column<int>(type: "int", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TargetEndDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Projects", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Projects_Users_PmId",
                        column: x => x.PmId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Main_Tasks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProjectId = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Progress = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    TargetDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ActualEndDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Variance = table.Column<int>(type: "int", nullable: true),
                    Mandays = table.Column<int>(type: "int", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    AssigneeId = table.Column<int>(type: "int", nullable: true),
                    QaAssigneeId = table.Column<int>(type: "int", nullable: true),
                    PhaseId = table.Column<int>(type: "int", nullable: false),
                    StatusId = table.Column<int>(type: "int", nullable: false, defaultValue: 1),
                    SeverityId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Main_Tasks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Main_Tasks_Phases_PhaseId",
                        column: x => x.PhaseId,
                        principalTable: "Phases",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Main_Tasks_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Main_Tasks_Severities_SeverityId",
                        column: x => x.SeverityId,
                        principalTable: "Severities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Main_Tasks_Statuses_StatusId",
                        column: x => x.StatusId,
                        principalTable: "Statuses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Main_Tasks_Users_AssigneeId",
                        column: x => x.AssigneeId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Main_Tasks_Users_QaAssigneeId",
                        column: x => x.QaAssigneeId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Project_Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProjectId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    Role = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Project_Users", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Project_Users_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Project_Users_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Activity_Log",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Main_TaskId = table.Column<int>(type: "int", nullable: true),
                    UserId = table.Column<int>(type: "int", nullable: true),
                    ActionDone = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Activity_Log", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Activity_Log_Main_Tasks_Main_TaskId",
                        column: x => x.Main_TaskId,
                        principalTable: "Main_Tasks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Activity_Log_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "Sub_Tasks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsDone = table.Column<int>(type: "int", nullable: false),
                    Main_TaskId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sub_Tasks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Sub_Tasks_Main_Tasks_Main_TaskId",
                        column: x => x.Main_TaskId,
                        principalTable: "Main_Tasks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Task_Attachments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Main_TaskId = table.Column<int>(type: "int", nullable: false),
                    Filename = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OriginalName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Mimetype = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Size = table.Column<int>(type: "int", nullable: false),
                    UploadedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Task_Attachments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Task_Attachments_Main_Tasks_Main_TaskId",
                        column: x => x.Main_TaskId,
                        principalTable: "Main_Tasks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Sub_Task_Comments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Sub_TaskId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: true),
                    Comment = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CommentDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sub_Task_Comments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Sub_Task_Comments_Sub_Tasks_Sub_TaskId",
                        column: x => x.Sub_TaskId,
                        principalTable: "Sub_Tasks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Sub_Task_Comments_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "Assessments",
                columns: new[] { "Id", "Label", "SortOrder" },
                values: new object[,]
                {
                    { 1, "Existing", 1 },
                    { 2, "Development / Customization", 2 },
                    { 3, "Enhancement", 3 },
                    { 4, "Not Applicable", 4 },
                    { 5, "Out of Scope", 5 },
                    { 6, "Defect", 6 }
                });

            migrationBuilder.InsertData(
                table: "Phases",
                columns: new[] { "Id", "Grouping", "IsDefault", "IsFinal", "Label", "SortOrder" },
                values: new object[] { 1, "dev", 1, 0, "Backlog (Requirements)", 1 });

            migrationBuilder.InsertData(
                table: "Phases",
                columns: new[] { "Id", "Grouping", "IsFinal", "Label", "SortOrder" },
                values: new object[,]
                {
                    { 2, "dev", 0, "To Do (Ready for Dev)", 2 },
                    { 3, "dev", 0, "In Progress", 3 },
                    { 4, "dev", 0, "For Review (Dev Done)", 4 },
                    { 5, "qa", 0, "Client Review - UAT", 5 },
                    { 6, "qa", 0, "QA Execution", 6 },
                    { 7, "qa", 0, "Deployed (Go-Live)", 7 },
                    { 8, "qa", 1, "Completed", 8 }
                });

            migrationBuilder.InsertData(
                table: "Severities",
                columns: new[] { "Id", "Color", "Label", "SortOrder" },
                values: new object[,]
                {
                    { 1, null, "1 - Critical / Showstopper", 1 },
                    { 2, null, "2 - High", 2 },
                    { 3, null, "3 - Medium", 3 },
                    { 4, null, "4 - Low", 4 },
                    { 5, null, "5 - Cosmetic Fix", 5 },
                    { 6, null, "Nice to Have", 6 }
                });

            migrationBuilder.InsertData(
                table: "Statuses",
                columns: new[] { "Id", "Color", "IsDefault", "Label", "SortOrder" },
                values: new object[] { 1, "#94a3b8", 1, "Not Started", 1 });

            migrationBuilder.InsertData(
                table: "Statuses",
                columns: new[] { "Id", "Color", "Label", "SortOrder" },
                values: new object[,]
                {
                    { 2, "#3b82f6", "Active", 2 },
                    { 3, "#ef4444", "Blocked", 3 },
                    { 4, "#f97316", "Bug Fixing", 4 },
                    { 5, "#f59e0b", "Clarification Needed", 5 },
                    { 6, "#8b5cf6", "For Verification", 6 },
                    { 7, "#dc2626", "Failed", 7 }
                });

            migrationBuilder.InsertData(
                table: "Statuses",
                columns: new[] { "Id", "Color", "IsFinal", "Label", "SortOrder" },
                values: new object[] { 8, "#22c55e", 1, "Passed", 8 });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "IsActive", "Name", "Password", "Role", "Username" },
                values: new object[] { 1, 1, "Admin User", "AQAAAAIAAYagAAAAEI88UJCZYAeB6zVYAb3w/j29tn4IEBWuoYQGSF2K6AzHYttOvYDsl+fc31yGv7B4tQ==", "Admin", "admin" });

            migrationBuilder.CreateIndex(
                name: "IX_Activity_Log_Main_TaskId",
                table: "Activity_Log",
                column: "Main_TaskId");

            migrationBuilder.CreateIndex(
                name: "IX_Activity_Log_UserId",
                table: "Activity_Log",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Assessments_Label",
                table: "Assessments",
                column: "Label",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Main_Tasks_AssigneeId",
                table: "Main_Tasks",
                column: "AssigneeId");

            migrationBuilder.CreateIndex(
                name: "IX_Main_Tasks_PhaseId",
                table: "Main_Tasks",
                column: "PhaseId");

            migrationBuilder.CreateIndex(
                name: "IX_Main_Tasks_ProjectId",
                table: "Main_Tasks",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_Main_Tasks_QaAssigneeId",
                table: "Main_Tasks",
                column: "QaAssigneeId");

            migrationBuilder.CreateIndex(
                name: "IX_Main_Tasks_SeverityId",
                table: "Main_Tasks",
                column: "SeverityId");

            migrationBuilder.CreateIndex(
                name: "IX_Main_Tasks_StatusId",
                table: "Main_Tasks",
                column: "StatusId");

            migrationBuilder.CreateIndex(
                name: "IX_Phases_Label",
                table: "Phases",
                column: "Label",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Project_Users_ProjectId",
                table: "Project_Users",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_Project_Users_UserId",
                table: "Project_Users",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Projects_PmId",
                table: "Projects",
                column: "PmId");

            migrationBuilder.CreateIndex(
                name: "IX_Projects_Title",
                table: "Projects",
                column: "Title",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Severities_Label",
                table: "Severities",
                column: "Label",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Statuses_Label",
                table: "Statuses",
                column: "Label",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Sub_Task_Comments_Sub_TaskId",
                table: "Sub_Task_Comments",
                column: "Sub_TaskId");

            migrationBuilder.CreateIndex(
                name: "IX_Sub_Task_Comments_UserId",
                table: "Sub_Task_Comments",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Sub_Tasks_Main_TaskId",
                table: "Sub_Tasks",
                column: "Main_TaskId");

            migrationBuilder.CreateIndex(
                name: "IX_Task_Attachments_Main_TaskId",
                table: "Task_Attachments",
                column: "Main_TaskId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Username",
                table: "Users",
                column: "Username",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Activity_Log");

            migrationBuilder.DropTable(
                name: "Assessments");

            migrationBuilder.DropTable(
                name: "Holidays");

            migrationBuilder.DropTable(
                name: "Project_Users");

            migrationBuilder.DropTable(
                name: "Sub_Task_Comments");

            migrationBuilder.DropTable(
                name: "Task_Attachments");

            migrationBuilder.DropTable(
                name: "Sub_Tasks");

            migrationBuilder.DropTable(
                name: "Main_Tasks");

            migrationBuilder.DropTable(
                name: "Phases");

            migrationBuilder.DropTable(
                name: "Projects");

            migrationBuilder.DropTable(
                name: "Severities");

            migrationBuilder.DropTable(
                name: "Statuses");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
