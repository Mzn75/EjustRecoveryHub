using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EjustRecoveryHub.Migrations
{
    /// <inheritdoc />
    public partial class AddContactEmail : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ContactEmail",
                table: "Items",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ContactEmail",
                table: "Items");
        }
    }
}
