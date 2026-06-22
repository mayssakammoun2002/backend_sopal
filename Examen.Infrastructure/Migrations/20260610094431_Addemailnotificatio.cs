using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Examen.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Addemailnotificatio : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_HistoriqueNotifications_Utilisateurs_UtilisateurId",
                table: "HistoriqueNotifications");

            migrationBuilder.DropIndex(
                name: "IX_HistoriqueNotifications_UtilisateurId",
                table: "HistoriqueNotifications");

            migrationBuilder.AlterColumn<string>(
                name: "Sujet",
                table: "HistoriqueNotifications",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255,
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Sujet",
                table: "HistoriqueNotifications",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255);

            migrationBuilder.CreateIndex(
                name: "IX_HistoriqueNotifications_UtilisateurId",
                table: "HistoriqueNotifications",
                column: "UtilisateurId");

            migrationBuilder.AddForeignKey(
                name: "FK_HistoriqueNotifications_Utilisateurs_UtilisateurId",
                table: "HistoriqueNotifications",
                column: "UtilisateurId",
                principalTable: "Utilisateurs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
