using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Examen.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Addemailnotifications : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Alertes_Machines_CodeMachine",
                table: "Alertes");

            migrationBuilder.DropForeignKey(
                name: "FK_Alertes_Seuils_SeuilId",
                table: "Alertes");

            migrationBuilder.DropForeignKey(
                name: "FK_HistoriqueNotifications_Utilisateurs_UtilisateurId",
                table: "HistoriqueNotifications");

            migrationBuilder.AlterColumn<decimal>(
                name: "TauxDetecte",
                table: "Alertes",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(5,2)");

            migrationBuilder.AlterColumn<int>(
                name: "Statut",
                table: "Alertes",
                type: "int",
                nullable: false,
                oldClrType: typeof(byte),
                oldType: "tinyint");

            migrationBuilder.AlterColumn<int>(
                name: "SeuilId",
                table: "Alertes",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "Niveau",
                table: "Alertes",
                type: "int",
                nullable: false,
                oldClrType: typeof(byte),
                oldType: "tinyint");

            migrationBuilder.AlterColumn<string>(
                name: "Message",
                table: "Alertes",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500);

            migrationBuilder.AlterColumn<string>(
                name: "CommentaireResolution",
                table: "Alertes",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(1000)",
                oldMaxLength: 1000,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CodeMachine",
                table: "Alertes",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20);

            migrationBuilder.AlterColumn<string>(
                name: "CodeArticle",
                table: "Alertes",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20,
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "NbNonConformesConsecutifs",
                table: "Alertes",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "NumOF",
                table: "Alertes",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Alertes_Seuils_SeuilId",
                table: "Alertes",
                column: "SeuilId",
                principalTable: "Seuils",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_HistoriqueNotifications_Utilisateurs_UtilisateurId",
                table: "HistoriqueNotifications",
                column: "UtilisateurId",
                principalTable: "Utilisateurs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Alertes_Seuils_SeuilId",
                table: "Alertes");

            migrationBuilder.DropForeignKey(
                name: "FK_HistoriqueNotifications_Utilisateurs_UtilisateurId",
                table: "HistoriqueNotifications");

            migrationBuilder.DropColumn(
                name: "NbNonConformesConsecutifs",
                table: "Alertes");

            migrationBuilder.DropColumn(
                name: "NumOF",
                table: "Alertes");

            migrationBuilder.AlterColumn<decimal>(
                name: "TauxDetecte",
                table: "Alertes",
                type: "decimal(5,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<byte>(
                name: "Statut",
                table: "Alertes",
                type: "tinyint",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "SeuilId",
                table: "Alertes",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<byte>(
                name: "Niveau",
                table: "Alertes",
                type: "tinyint",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "Message",
                table: "Alertes",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "CommentaireResolution",
                table: "Alertes",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CodeMachine",
                table: "Alertes",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "CodeArticle",
                table: "Alertes",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Alertes_Machines_CodeMachine",
                table: "Alertes",
                column: "CodeMachine",
                principalTable: "Machines",
                principalColumn: "CodeMachine",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Alertes_Seuils_SeuilId",
                table: "Alertes",
                column: "SeuilId",
                principalTable: "Seuils",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_HistoriqueNotifications_Utilisateurs_UtilisateurId",
                table: "HistoriqueNotifications",
                column: "UtilisateurId",
                principalTable: "Utilisateurs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
