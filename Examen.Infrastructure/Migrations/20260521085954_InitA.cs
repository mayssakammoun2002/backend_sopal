using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Examen.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitA : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Alertes_TypeDefauts_TypeDefaut1Id",
                table: "Alertes");

            migrationBuilder.DropForeignKey(
                name: "FK_Alertes_TypeDefauts_TypeDefaut2Id",
                table: "Alertes");

            migrationBuilder.DropIndex(
                name: "IX_Alertes_TypeDefaut1Id",
                table: "Alertes");

            migrationBuilder.DropIndex(
                name: "IX_Alertes_TypeDefaut2Id",
                table: "Alertes");

            migrationBuilder.DropColumn(
                name: "SeuilQuantite",
                table: "Seuils");

            migrationBuilder.DropColumn(
                name: "MachineId",
                table: "Alertes");

            migrationBuilder.DropColumn(
                name: "TypeDefaut1Id",
                table: "Alertes");

            migrationBuilder.DropColumn(
                name: "TypeDefaut2Id",
                table: "Alertes");

            migrationBuilder.AlterColumn<string>(
                name: "Message",
                table: "Alertes",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "CommentairesAlerte",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AlerteId = table.Column<int>(type: "int", nullable: false),
                    AuteurId = table.Column<int>(type: "int", nullable: false),
                    NomAuteur = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Contenu = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    DateCreation = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommentairesAlerte", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CommentairesAlerte_Alertes_AlerteId",
                        column: x => x.AlerteId,
                        principalTable: "Alertes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CommentairesAlerte_AlerteId",
                table: "CommentairesAlerte",
                column: "AlerteId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CommentairesAlerte");

            migrationBuilder.AddColumn<int>(
                name: "SeuilQuantite",
                table: "Seuils",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Message",
                table: "Alertes",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500);

            migrationBuilder.AddColumn<string>(
                name: "MachineId",
                table: "Alertes",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TypeDefaut1Id",
                table: "Alertes",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TypeDefaut2Id",
                table: "Alertes",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Alertes_TypeDefaut1Id",
                table: "Alertes",
                column: "TypeDefaut1Id");

            migrationBuilder.CreateIndex(
                name: "IX_Alertes_TypeDefaut2Id",
                table: "Alertes",
                column: "TypeDefaut2Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Alertes_TypeDefauts_TypeDefaut1Id",
                table: "Alertes",
                column: "TypeDefaut1Id",
                principalTable: "TypeDefauts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Alertes_TypeDefauts_TypeDefaut2Id",
                table: "Alertes",
                column: "TypeDefaut2Id",
                principalTable: "TypeDefauts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
