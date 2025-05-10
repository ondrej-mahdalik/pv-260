using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PV260.API.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddCreatedAtIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Reports_CreatedAt",
                table: "Reports",
                column: "CreatedAt",
                descending: new bool[0]);

            migrationBuilder.CreateIndex(
                name: "IX_Emails_CreatedAt",
                table: "Emails",
                column: "CreatedAt",
                descending: new bool[0]);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Reports_CreatedAt",
                table: "Reports");

            migrationBuilder.DropIndex(
                name: "IX_Emails_CreatedAt",
                table: "Emails");
        }
    }
}
