using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QtechOJT_Net9.Migrations
{
    /// <inheritdoc />
    public partial class CreatorIdsSupport : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CreatorId",
                table: "Sub_Tasks",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CreatorId",
                table: "Main_Tasks",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Sub_Tasks_CreatorId",
                table: "Sub_Tasks",
                column: "CreatorId");

            migrationBuilder.CreateIndex(
                name: "IX_Main_Tasks_CreatorId",
                table: "Main_Tasks",
                column: "CreatorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Main_Tasks_Users_CreatorId",
                table: "Main_Tasks",
                column: "CreatorId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Sub_Tasks_Users_CreatorId",
                table: "Sub_Tasks",
                column: "CreatorId",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Main_Tasks_Users_CreatorId",
                table: "Main_Tasks");

            migrationBuilder.DropForeignKey(
                name: "FK_Sub_Tasks_Users_CreatorId",
                table: "Sub_Tasks");

            migrationBuilder.DropIndex(
                name: "IX_Sub_Tasks_CreatorId",
                table: "Sub_Tasks");

            migrationBuilder.DropIndex(
                name: "IX_Main_Tasks_CreatorId",
                table: "Main_Tasks");

            migrationBuilder.DropColumn(
                name: "CreatorId",
                table: "Sub_Tasks");

            migrationBuilder.DropColumn(
                name: "CreatorId",
                table: "Main_Tasks");
        }
    }
}
