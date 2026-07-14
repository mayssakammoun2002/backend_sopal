using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Examen.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AjoutUploadDownloadProfilFonctionDroit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Download",
                table: "ProfilFonctionDroits",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Upload",
                table: "ProfilFonctionDroits",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Download",
                table: "ProfilFonctionDroits");

            migrationBuilder.DropColumn(
                name: "Upload",
                table: "ProfilFonctionDroits");
        }
    }
}
