using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Examen.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Addlotsss : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "LotId",
                table: "ResultatControles",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Statut",
                table: "Machines",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "LotId",
                table: "Alertes",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Lot",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NumeroLot = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    MachineId = table.Column<string>(type: "nvarchar(20)", nullable: false),
                    ProduitId = table.Column<string>(type: "nvarchar(20)", nullable: false),
                    OperateurId = table.Column<int>(type: "int", nullable: false),
                    DateDebut = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateFin = table.Column<DateTime>(type: "datetime2", nullable: true),
                    QuantitePrevue = table.Column<int>(type: "int", nullable: false),
                    QuantiteProduite = table.Column<int>(type: "int", nullable: true),
                    Statut = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Commentaire = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Lot", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Lot_Machines_MachineId",
                        column: x => x.MachineId,
                        principalTable: "Machines",
                        principalColumn: "CodeMachine",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Lot_Produits_ProduitId",
                        column: x => x.ProduitId,
                        principalTable: "Produits",
                        principalColumn: "CodeArticle",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Lot_Utilisateurs_OperateurId",
                        column: x => x.OperateurId,
                        principalTable: "Utilisateurs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ResultatControles_LotId",
                table: "ResultatControles",
                column: "LotId");

            migrationBuilder.CreateIndex(
                name: "IX_Alertes_LotId",
                table: "Alertes",
                column: "LotId");

            migrationBuilder.CreateIndex(
                name: "IX_Lot_MachineId",
                table: "Lot",
                column: "MachineId");

            migrationBuilder.CreateIndex(
                name: "IX_Lot_OperateurId",
                table: "Lot",
                column: "OperateurId");

            migrationBuilder.CreateIndex(
                name: "IX_Lot_ProduitId",
                table: "Lot",
                column: "ProduitId");

            migrationBuilder.AddForeignKey(
                name: "FK_Alertes_Lot_LotId",
                table: "Alertes",
                column: "LotId",
                principalTable: "Lot",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ResultatControles_Lot_LotId",
                table: "ResultatControles",
                column: "LotId",
                principalTable: "Lot",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Alertes_Lot_LotId",
                table: "Alertes");

            migrationBuilder.DropForeignKey(
                name: "FK_ResultatControles_Lot_LotId",
                table: "ResultatControles");

            migrationBuilder.DropTable(
                name: "Lot");

            migrationBuilder.DropIndex(
                name: "IX_ResultatControles_LotId",
                table: "ResultatControles");

            migrationBuilder.DropIndex(
                name: "IX_Alertes_LotId",
                table: "Alertes");

            migrationBuilder.DropColumn(
                name: "LotId",
                table: "ResultatControles");

            migrationBuilder.DropColumn(
                name: "Statut",
                table: "Machines");

            migrationBuilder.DropColumn(
                name: "LotId",
                table: "Alertes");
        }
    }
}
