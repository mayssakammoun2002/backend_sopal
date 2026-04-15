using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Examen.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Initsss : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Machines",
                columns: table => new
                {
                    CodeMachine = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    NomMachine = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Actif = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Machines", x => x.CodeMachine);
                });

            migrationBuilder.CreateTable(
                name: "Produits",
                columns: table => new
                {
                    CodeArticle = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    NomProduit = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Designation = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    TailleEchantillonnage = table.Column<int>(type: "int", nullable: false),
                    Cadence = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Produits", x => x.CodeArticle);
                });

            migrationBuilder.CreateTable(
                name: "TypeDefauts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NomDefaut = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    CauseProbable = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Solution = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Frequence = table.Column<int>(type: "int", nullable: false),
                    ImagePath = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TypeDefauts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Utilisateurs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FirstName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Role = table.Column<int>(type: "int", nullable: false),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Actif = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Utilisateurs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ResultatControles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: false),
                    DateControle = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CodeMachine = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    CodeArticle = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    UtilisateurId = table.Column<int>(type: "int", nullable: false),
                    NumOF = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Quantite = table.Column<int>(type: "int", nullable: false),
                    Cadence = table.Column<int>(type: "int", nullable: false),
                    NbEchantillons = table.Column<int>(type: "int", nullable: false),
                    StatutLot = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    SolutionGlobale = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    TypeDefaut1Id = table.Column<int>(type: "int", nullable: true),
                    TypeDefaut2Id = table.Column<int>(type: "int", nullable: true),
                    NbDefautsTest1 = table.Column<int>(type: "int", nullable: false),
                    NbDefautsTest2 = table.Column<int>(type: "int", nullable: false),
                    Defaut1 = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Defaut2 = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ResultatControles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ResultatControles_Machines_CodeMachine",
                        column: x => x.CodeMachine,
                        principalTable: "Machines",
                        principalColumn: "CodeMachine",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ResultatControles_Produits_CodeArticle",
                        column: x => x.CodeArticle,
                        principalTable: "Produits",
                        principalColumn: "CodeArticle",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ResultatControles_TypeDefauts_TypeDefaut1Id",
                        column: x => x.TypeDefaut1Id,
                        principalTable: "TypeDefauts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ResultatControles_TypeDefauts_TypeDefaut2Id",
                        column: x => x.TypeDefaut2Id,
                        principalTable: "TypeDefauts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ResultatControles_Utilisateurs_UtilisateurId",
                        column: x => x.UtilisateurId,
                        principalTable: "Utilisateurs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ResultatControles_CodeArticle",
                table: "ResultatControles",
                column: "CodeArticle");

            migrationBuilder.CreateIndex(
                name: "IX_ResultatControles_CodeMachine",
                table: "ResultatControles",
                column: "CodeMachine");

            migrationBuilder.CreateIndex(
                name: "IX_ResultatControles_TypeDefaut1Id",
                table: "ResultatControles",
                column: "TypeDefaut1Id");

            migrationBuilder.CreateIndex(
                name: "IX_ResultatControles_TypeDefaut2Id",
                table: "ResultatControles",
                column: "TypeDefaut2Id");

            migrationBuilder.CreateIndex(
                name: "IX_ResultatControles_UtilisateurId",
                table: "ResultatControles",
                column: "UtilisateurId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ResultatControles");

            migrationBuilder.DropTable(
                name: "Machines");

            migrationBuilder.DropTable(
                name: "Produits");

            migrationBuilder.DropTable(
                name: "TypeDefauts");

            migrationBuilder.DropTable(
                name: "Utilisateurs");
        }
    }
}
