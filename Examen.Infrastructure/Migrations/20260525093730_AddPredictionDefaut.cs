using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Examen.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPredictionDefaut : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MessageErreur",
                table: "HistoriqueNotifications");

            migrationBuilder.CreateTable(
                name: "PredictionsDefauts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DatePrediction = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ResultatControleId = table.Column<string>(type: "nvarchar(36)", nullable: false),
                    EstDefectueux = table.Column<bool>(type: "bit", nullable: false),
                    Probabilite = table.Column<double>(type: "float", nullable: false),
                    TypeDefautPreditId = table.Column<int>(type: "int", nullable: true),
                    NiveauRisque = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    UtilisateurId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PredictionsDefauts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PredictionsDefauts_ResultatControles_ResultatControleId",
                        column: x => x.ResultatControleId,
                        principalTable: "ResultatControles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PredictionsDefauts_TypeDefauts_TypeDefautPreditId",
                        column: x => x.TypeDefautPreditId,
                        principalTable: "TypeDefauts",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PredictionsDefauts_Utilisateurs_UtilisateurId",
                        column: x => x.UtilisateurId,
                        principalTable: "Utilisateurs",
                        principalColumn: "Id");
                });

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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PredictionsDefauts");

            migrationBuilder.AddColumn<string>(
                name: "MessageErreur",
                table: "HistoriqueNotifications",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
