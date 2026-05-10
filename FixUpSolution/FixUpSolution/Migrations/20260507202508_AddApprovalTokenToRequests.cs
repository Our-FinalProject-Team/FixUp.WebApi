using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FixUpSolution.Migrations
{
    /// <inheritdoc />
    public partial class AddApprovalTokenToRequests : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ApprovalToken",
                table: "Requests",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ApprovalToken",
                table: "Requests");
        }
    }
}
