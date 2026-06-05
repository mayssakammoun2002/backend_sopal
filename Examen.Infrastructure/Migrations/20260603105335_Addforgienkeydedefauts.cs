using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Examen.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Addforgienkeydedefauts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PredictionsDefauts_ResultatControles_ResultatControleId",
                table: "PredictionsDefauts");

            migrationBuilder.DropForeignKey(
                name: "FK_PredictionsDefauts_TypeDefauts_TypeDefautPreditId",
                table: "PredictionsDefauts");

            migrationBuilder.DropForeignKey(
                name: "FK_PredictionsDefauts_Utilisateurs_UtilisateurId",
                table: "PredictionsDefauts");

            migrationBuilder.DropIndex(
                name: "IX_PredictionsDefauts_ResultatControleId",
                table: "PredictionsDefauts");

            migrationBuilder.DropIndex(
                name: "IX_PredictionsDefauts_TypeDefautPreditId",
                table: "PredictionsDefauts");

            migrationBuilder.DropIndex(
                name: "IX_PredictionsDefauts_UtilisateurId",
                table: "PredictionsDefauts");

            migrationBuilder.RenameColumn(
                name: "UtilisateurId",
                table: "PredictionsDefauts",
                newName: "LatenceMs");

            migrationBuilder.AlterColumn<string>(
                name: "ResultatControleId",
                table: "PredictionsDefauts",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(36)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Probabilite",
                table: "PredictionsDefauts",
                type: "decimal(5,4)",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.AddColumn<string>(
                name: "FeaturesJson",
                table: "PredictionsDefauts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ModelVersion",
                table: "PredictionsDefauts",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ShapExplicationJson",
                table: "PredictionsDefauts",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FeaturesJson",
                table: "PredictionsDefauts");

            migrationBuilder.DropColumn(
                name: "ModelVersion",
                table: "PredictionsDefauts");

            migrationBuilder.DropColumn(
                name: "ShapExplicationJson",
                table: "PredictionsDefauts");

            migrationBuilder.RenameColumn(
                name: "LatenceMs",
                table: "PredictionsDefauts",
                newName: "UtilisateurId");

            migrationBuilder.AlterColumn<string>(
                name: "ResultatControleId",
                table: "PredictionsDefauts",
                type: "nvarchar(36)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<double>(
                name: "Probabilite",
                table: "PredictionsDefauts",
                type: "float",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(5,4)");

            migrationBuilder.CreateIndex(
                name: "IX_PredictionsDefauts_ResultatControleId",
                table: "PredictionsDefauts",
                column: "ResultatControleId");

            migrationBuilder.CreateIndex(
                name: "IX_PredictionsDefauts_TypeDefautPreditId",
                table: "PredictionsDefauts",
                column: "TypeDefautPreditId");

            migrationBuilder.CreateIndex(
                name: "IX_PredictionsDefauts_UtilisateurId",
                table: "PredictionsDefauts",
                column: "UtilisateurId");

            migrationBuilder.AddForeignKey(
                name: "FK_PredictionsDefauts_ResultatControles_ResultatControleId",
                table: "PredictionsDefauts",
                column: "ResultatControleId",
                principalTable: "ResultatControles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PredictionsDefauts_TypeDefauts_TypeDefautPreditId",
                table: "PredictionsDefauts",
                column: "TypeDefautPreditId",
                principalTable: "TypeDefauts",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PredictionsDefauts_Utilisateurs_UtilisateurId",
                table: "PredictionsDefauts",
                column: "UtilisateurId",
                principalTable: "Utilisateurs",
                principalColumn: "Id");
        }
    }
}
