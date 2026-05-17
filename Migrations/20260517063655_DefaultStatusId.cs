using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QtechOJT_Net9.Migrations
{
    /// <inheritdoc />
    public partial class DefaultStatusId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DefaultStatusId",
                table: "Phases",
                type: "int",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Phases",
                keyColumn: "Id",
                keyValue: 1,
                column: "DefaultStatusId",
                value: null);

            migrationBuilder.UpdateData(
                table: "Phases",
                keyColumn: "Id",
                keyValue: 2,
                column: "DefaultStatusId",
                value: null);

            migrationBuilder.UpdateData(
                table: "Phases",
                keyColumn: "Id",
                keyValue: 3,
                column: "DefaultStatusId",
                value: null);

            migrationBuilder.UpdateData(
                table: "Phases",
                keyColumn: "Id",
                keyValue: 4,
                column: "DefaultStatusId",
                value: null);

            migrationBuilder.UpdateData(
                table: "Phases",
                keyColumn: "Id",
                keyValue: 5,
                column: "DefaultStatusId",
                value: null);

            migrationBuilder.UpdateData(
                table: "Phases",
                keyColumn: "Id",
                keyValue: 6,
                column: "DefaultStatusId",
                value: null);

            migrationBuilder.UpdateData(
                table: "Phases",
                keyColumn: "Id",
                keyValue: 7,
                column: "DefaultStatusId",
                value: null);

            migrationBuilder.UpdateData(
                table: "Phases",
                keyColumn: "Id",
                keyValue: 8,
                column: "DefaultStatusId",
                value: null);

            migrationBuilder.CreateIndex(
                name: "IX_Phases_DefaultStatusId",
                table: "Phases",
                column: "DefaultStatusId");

            migrationBuilder.AddForeignKey(
                name: "FK_Phases_Statuses_DefaultStatusId",
                table: "Phases",
                column: "DefaultStatusId",
                principalTable: "Statuses",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Phases_Statuses_DefaultStatusId",
                table: "Phases");

            migrationBuilder.DropIndex(
                name: "IX_Phases_DefaultStatusId",
                table: "Phases");

            migrationBuilder.DropColumn(
                name: "DefaultStatusId",
                table: "Phases");
        }
    }
}
