using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QtechOJT_Net9.Migrations
{
    /// <inheritdoc />
    public partial class Added_StartDate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "StartDate",
                table: "Main_Tasks",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StartDate",
                table: "Main_Tasks");
        }
    }
}
