using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FixUpSolution.Migrations
{
    /// <inheritdoc />
    public partial class UpdateRequestTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ImageUrl",
                table: "Requests",
                newName: "Address");

            migrationBuilder.AddColumn<DateTime>(
                name: "ScheduledDate",
                table: "Requests",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ScheduledDate",
                table: "Requests");

            migrationBuilder.RenameColumn(
                name: "Address",
                table: "Requests",
                newName: "ImageUrl");
        }
    }
}
