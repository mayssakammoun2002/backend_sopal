using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Examen.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Ajout_Notifications : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DestinatairesNotification",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UtilisateurId = table.Column<int>(type: "int", nullable: true),
                    Role = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Canal = table.Column<byte>(type: "tinyint", nullable: false),
                    NiveauMinimum = table.Column<byte>(type: "tinyint", nullable: false),
                    EstActif = table.Column<bool>(type: "bit", nullable: false),
                    Destinataire = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DestinatairesNotification", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DestinatairesNotification_Utilisateurs_UtilisateurId",
                        column: x => x.UtilisateurId,
                        principalTable: "Utilisateurs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Seuils",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CodeMachine = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    CodeArticle = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    TypeDefaut1Id = table.Column<int>(type: "int", nullable: true),
                    SeuilPourcentage = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    SeuilQuantite = table.Column<int>(type: "int", nullable: true),
                    EstActif = table.Column<bool>(type: "bit", nullable: false),
                    DateCreation = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateModification = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreePar = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Seuils", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Seuils_Machines_CodeMachine",
                        column: x => x.CodeMachine,
                        principalTable: "Machines",
                        principalColumn: "CodeMachine",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Seuils_Produits_CodeArticle",
                        column: x => x.CodeArticle,
                        principalTable: "Produits",
                        principalColumn: "CodeArticle",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Seuils_TypeDefauts_TypeDefaut1Id",
                        column: x => x.TypeDefaut1Id,
                        principalTable: "TypeDefauts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Alertes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SeuilId = table.Column<int>(type: "int", nullable: false),
                    CodeMachine = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    CodeArticle = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    TypeDefaut1Id = table.Column<int>(type: "int", nullable: true),
                    TypeDefaut2Id = table.Column<int>(type: "int", nullable: true),
                    TauxDetecte = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    QuantiteDefauts = table.Column<int>(type: "int", nullable: false),
                    QuantiteTotale = table.Column<int>(type: "int", nullable: false),
                    Niveau = table.Column<byte>(type: "tinyint", nullable: false),
                    Statut = table.Column<byte>(type: "tinyint", nullable: false),
                    Message = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    DateAlerte = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateResolution = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ResolueParId = table.Column<int>(type: "int", nullable: true),
                    CommentaireResolution = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Alertes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Alertes_Machines_CodeMachine",
                        column: x => x.CodeMachine,
                        principalTable: "Machines",
                        principalColumn: "CodeMachine",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Alertes_Seuils_SeuilId",
                        column: x => x.SeuilId,
                        principalTable: "Seuils",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Alertes_TypeDefauts_TypeDefaut1Id",
                        column: x => x.TypeDefaut1Id,
                        principalTable: "TypeDefauts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Alertes_TypeDefauts_TypeDefaut2Id",
                        column: x => x.TypeDefaut2Id,
                        principalTable: "TypeDefauts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Alertes_Utilisateurs_ResolueParId",
                        column: x => x.ResolueParId,
                        principalTable: "Utilisateurs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "HistoriqueNotifications",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AlerteId = table.Column<int>(type: "int", nullable: false),
                    UtilisateurId = table.Column<int>(type: "int", nullable: true),
                    Canal = table.Column<byte>(type: "tinyint", nullable: false),
                    Destinataire = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Sujet = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Corps = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Statut = table.Column<byte>(type: "tinyint", nullable: false),
                    DateEnvoi = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DateLecture = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ErreurMessage = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    NbTentatives = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HistoriqueNotifications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HistoriqueNotifications_Alertes_AlerteId",
                        column: x => x.AlerteId,
                        principalTable: "Alertes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_HistoriqueNotifications_Utilisateurs_UtilisateurId",
                        column: x => x.UtilisateurId,
                        principalTable: "Utilisateurs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Alertes_CodeMachine",
                table: "Alertes",
                column: "CodeMachine");

            migrationBuilder.CreateIndex(
                name: "IX_Alertes_DateAlerte",
                table: "Alertes",
                column: "DateAlerte");

            migrationBuilder.CreateIndex(
                name: "IX_Alertes_ResolueParId",
                table: "Alertes",
                column: "ResolueParId");

            migrationBuilder.CreateIndex(
                name: "IX_Alertes_SeuilId",
                table: "Alertes",
                column: "SeuilId");

            migrationBuilder.CreateIndex(
                name: "IX_Alertes_Statut",
                table: "Alertes",
                column: "Statut");

            migrationBuilder.CreateIndex(
                name: "IX_Alertes_TypeDefaut1Id",
                table: "Alertes",
                column: "TypeDefaut1Id");

            migrationBuilder.CreateIndex(
                name: "IX_Alertes_TypeDefaut2Id",
                table: "Alertes",
                column: "TypeDefaut2Id");

            migrationBuilder.CreateIndex(
                name: "IX_DestinatairesNotification_UtilisateurId",
                table: "DestinatairesNotification",
                column: "UtilisateurId");

            migrationBuilder.CreateIndex(
                name: "IX_HistoriqueNotifications_AlerteId",
                table: "HistoriqueNotifications",
                column: "AlerteId");

            migrationBuilder.CreateIndex(
                name: "IX_HistoriqueNotifications_Statut",
                table: "HistoriqueNotifications",
                column: "Statut");

            migrationBuilder.CreateIndex(
                name: "IX_HistoriqueNotifications_UtilisateurId",
                table: "HistoriqueNotifications",
                column: "UtilisateurId");

            migrationBuilder.CreateIndex(
                name: "IX_Seuils_CodeArticle",
                table: "Seuils",
                column: "CodeArticle");

            migrationBuilder.CreateIndex(
                name: "IX_Seuils_CodeMachine",
                table: "Seuils",
                column: "CodeMachine");

            migrationBuilder.CreateIndex(
                name: "IX_Seuils_TypeDefaut1Id",
                table: "Seuils",
                column: "TypeDefaut1Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DestinatairesNotification");

            migrationBuilder.DropTable(
                name: "HistoriqueNotifications");

            migrationBuilder.DropTable(
                name: "Alertes");

            migrationBuilder.DropTable(
                name: "Seuils");
        }
    }
}
