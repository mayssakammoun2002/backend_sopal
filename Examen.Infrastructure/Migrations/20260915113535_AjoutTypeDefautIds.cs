using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Examen.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AjoutTypeDefautIds : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ControleurIds",
                table: "ResultatControles",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ControleurNoms",
                table: "ResultatControles",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NumConteneur",
                table: "ResultatControles",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ControleurIds",
                table: "ResultatControles");

            migrationBuilder.DropColumn(
                name: "ControleurNoms",
                table: "ResultatControles");

            migrationBuilder.DropColumn(
                name: "NumConteneur",
                table: "ResultatControles");
        }
    }
}
