using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Examen.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FKProduitOptionnelleeo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ResultatControles_Produits_CodeArticle",
                table: "ResultatControles");

            migrationBuilder.DropIndex(
                name: "IX_ResultatControles_CodeArticle",
                table: "ResultatControles");

            migrationBuilder.AddColumn<string>(
                name: "ProduitCodeArticle",
                table: "ResultatControles",
                type: "nvarchar(20)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ResultatControles_ProduitCodeArticle",
                table: "ResultatControles",
                column: "ProduitCodeArticle");

            migrationBuilder.AddForeignKey(
                name: "FK_ResultatControles_Produits_ProduitCodeArticle",
                table: "ResultatControles",
                column: "ProduitCodeArticle",
                principalTable: "Produits",
                principalColumn: "CodeArticle");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ResultatControles_Produits_ProduitCodeArticle",
                table: "ResultatControles");

            migrationBuilder.DropIndex(
                name: "IX_ResultatControles_ProduitCodeArticle",
                table: "ResultatControles");

            migrationBuilder.DropColumn(
                name: "ProduitCodeArticle",
                table: "ResultatControles");

            migrationBuilder.CreateIndex(
                name: "IX_ResultatControles_CodeArticle",
                table: "ResultatControles",
                column: "CodeArticle");

            migrationBuilder.AddForeignKey(
                name: "FK_ResultatControles_Produits_CodeArticle",
                table: "ResultatControles",
                column: "CodeArticle",
                principalTable: "Produits",
                principalColumn: "CodeArticle",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
