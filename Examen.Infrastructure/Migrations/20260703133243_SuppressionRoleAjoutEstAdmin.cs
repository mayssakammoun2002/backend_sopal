using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Examen.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SuppressionRoleAjoutEstAdmin : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Role",
                table: "Utilisateurs");

            migrationBuilder.AddColumn<bool>(
                name: "EstAdmin",
                table: "Profils",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EstAdmin",
                table: "Profils");

            migrationBuilder.AddColumn<int>(
                name: "Role",
                table: "Utilisateurs",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
