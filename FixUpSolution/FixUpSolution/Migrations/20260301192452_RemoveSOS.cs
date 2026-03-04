using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FixUpSolution.Migrations
{
    /// <inheritdoc />
    public partial class RemoveSOS : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CurrentLat",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "CurrentLng",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "IsActiveNow",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "LocationLat",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "LocationLng",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "Tasks");

            migrationBuilder.AlterColumn<DateTime>(
                name: "ScheduledDate",
                table: "Tasks",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "CurrentLat",
                table: "Users",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "CurrentLng",
                table: "Users",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActiveNow",
                table: "Users",
                type: "bit",
                nullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "ScheduledDate",
                table: "Tasks",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AddColumn<double>(
                name: "LocationLat",
                table: "Tasks",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "LocationLng",
                table: "Tasks",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<int>(
                name: "Type",
                table: "Tasks",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
