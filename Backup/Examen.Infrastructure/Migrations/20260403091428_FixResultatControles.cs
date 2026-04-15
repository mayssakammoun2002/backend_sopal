using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Examen.Infrastructure.Migrations
{
    public partial class FixResultatControles : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // ══════════════════════════════════════════
            // 1. SUPPRIMER TOUTES LES FK
            // ══════════════════════════════════════════

            migrationBuilder.Sql(@"
                IF EXISTS (SELECT 1 FROM sys.foreign_keys 
                           WHERE name = 'FK_ResultatControles_Produits_CodeArticle')
                    ALTER TABLE [ResultatControles] 
                    DROP CONSTRAINT [FK_ResultatControles_Produits_CodeArticle];
            ");

            migrationBuilder.Sql(@"
                IF EXISTS (SELECT 1 FROM sys.foreign_keys 
                           WHERE name = 'FK_ResultatControles_Machines_CodeMachine')
                    ALTER TABLE [ResultatControles] 
                    DROP CONSTRAINT [FK_ResultatControles_Machines_CodeMachine];
            ");

            migrationBuilder.Sql(@"
                IF EXISTS (SELECT 1 FROM sys.foreign_keys 
                           WHERE name = 'FK_ResultatControles_Utilisateurs_UtilisateurId')
                    ALTER TABLE [ResultatControles] 
                    DROP CONSTRAINT [FK_ResultatControles_Utilisateurs_UtilisateurId];
            ");

            migrationBuilder.Sql(@"
                IF EXISTS (SELECT 1 FROM sys.foreign_keys 
                           WHERE name = 'FK_ResultatControles_Utilisateurs_UtilisateurId1')
                    ALTER TABLE [ResultatControles] 
                    DROP CONSTRAINT [FK_ResultatControles_Utilisateurs_UtilisateurId1];
            ");

            // ══════════════════════════════════════════
            // 2. SUPPRIMER TOUS LES INDEX
            // ══════════════════════════════════════════

            migrationBuilder.Sql(@"
                IF EXISTS (SELECT 1 FROM sys.indexes 
                           WHERE name = 'IX_ResultatControles_CodeArticle'
                           AND object_id = OBJECT_ID('ResultatControles'))
                    DROP INDEX [IX_ResultatControles_CodeArticle] ON [ResultatControles];
            ");

            migrationBuilder.Sql(@"
                IF EXISTS (SELECT 1 FROM sys.indexes 
                           WHERE name = 'IX_ResultatControles_CodeMachine'
                           AND object_id = OBJECT_ID('ResultatControles'))
                    DROP INDEX [IX_ResultatControles_CodeMachine] ON [ResultatControles];
            ");

            migrationBuilder.Sql(@"
                IF EXISTS (SELECT 1 FROM sys.indexes 
                           WHERE name = 'IX_ResultatControles_UtilisateurId1'
                           AND object_id = OBJECT_ID('ResultatControles'))
                    DROP INDEX [IX_ResultatControles_UtilisateurId1] ON [ResultatControles];
            ");

            // ══════════════════════════════════════════
            // 3. SUPPRIMER LA CLÉ PRIMAIRE
            // ══════════════════════════════════════════

            migrationBuilder.Sql(@"
                IF EXISTS (SELECT 1 FROM sys.key_constraints 
                           WHERE name = 'PK_ResultatControles'
                           AND parent_object_id = OBJECT_ID('ResultatControles'))
                    ALTER TABLE [ResultatControles] 
                    DROP CONSTRAINT [PK_ResultatControles];
            ");

            // ══════════════════════════════════════════
            // 4. SUPPRIMER TOUTES LES CONTRAINTES DEFAULT
            //    sur chaque colonne à modifier
            //    (nom généré automatiquement par SQL Server
            //     ex: DF__ResultatC__NumOF__0B91BA14)
            // ══════════════════════════════════════════

            // ✅ Supprimer DEFAULT sur Id
            migrationBuilder.Sql(@"
                DECLARE @sql NVARCHAR(200)
                SELECT @sql = 'ALTER TABLE [ResultatControles] DROP CONSTRAINT ' + d.name
                FROM sys.default_constraints d
                JOIN sys.columns c ON d.parent_object_id = c.object_id 
                                   AND d.parent_column_id = c.column_id
                WHERE d.parent_object_id = OBJECT_ID('ResultatControles')
                  AND c.name = 'Id'
                IF @sql IS NOT NULL EXEC(@sql);
            ");

            // ✅ Supprimer DEFAULT sur CodeArticle
            migrationBuilder.Sql(@"
                DECLARE @sql NVARCHAR(200)
                SELECT @sql = 'ALTER TABLE [ResultatControles] DROP CONSTRAINT ' + d.name
                FROM sys.default_constraints d
                JOIN sys.columns c ON d.parent_object_id = c.object_id 
                                   AND d.parent_column_id = c.column_id
                WHERE d.parent_object_id = OBJECT_ID('ResultatControles')
                  AND c.name = 'CodeArticle'
                IF @sql IS NOT NULL EXEC(@sql);
            ");

            // ✅ Supprimer DEFAULT sur CodeMachine
            migrationBuilder.Sql(@"
                DECLARE @sql NVARCHAR(200)
                SELECT @sql = 'ALTER TABLE [ResultatControles] DROP CONSTRAINT ' + d.name
                FROM sys.default_constraints d
                JOIN sys.columns c ON d.parent_object_id = c.object_id 
                                   AND d.parent_column_id = c.column_id
                WHERE d.parent_object_id = OBJECT_ID('ResultatControles')
                  AND c.name = 'CodeMachine'
                IF @sql IS NOT NULL EXEC(@sql);
            ");

            // ✅ Supprimer DEFAULT sur NumOF  ← celui qui causait l'erreur
            migrationBuilder.Sql(@"
                DECLARE @sql NVARCHAR(200)
                SELECT @sql = 'ALTER TABLE [ResultatControles] DROP CONSTRAINT ' + d.name
                FROM sys.default_constraints d
                JOIN sys.columns c ON d.parent_object_id = c.object_id 
                                   AND d.parent_column_id = c.column_id
                WHERE d.parent_object_id = OBJECT_ID('ResultatControles')
                  AND c.name = 'NumOF'
                IF @sql IS NOT NULL EXEC(@sql);
            ");

            // ✅ Supprimer DEFAULT sur StatutLot
            migrationBuilder.Sql(@"
                DECLARE @sql NVARCHAR(200)
                SELECT @sql = 'ALTER TABLE [ResultatControles] DROP CONSTRAINT ' + d.name
                FROM sys.default_constraints d
                JOIN sys.columns c ON d.parent_object_id = c.object_id 
                                   AND d.parent_column_id = c.column_id
                WHERE d.parent_object_id = OBJECT_ID('ResultatControles')
                  AND c.name = 'StatutLot'
                IF @sql IS NOT NULL EXEC(@sql);
            ");

            // ✅ Supprimer DEFAULT sur SolutionGlobale
            migrationBuilder.Sql(@"
                DECLARE @sql NVARCHAR(200)
                SELECT @sql = 'ALTER TABLE [ResultatControles] DROP CONSTRAINT ' + d.name
                FROM sys.default_constraints d
                JOIN sys.columns c ON d.parent_object_id = c.object_id 
                                   AND d.parent_column_id = c.column_id
                WHERE d.parent_object_id = OBJECT_ID('ResultatControles')
                  AND c.name = 'SolutionGlobale'
                IF @sql IS NOT NULL EXEC(@sql);
            ");

            // ✅ Supprimer DEFAULT sur Defaut1
            migrationBuilder.Sql(@"
                DECLARE @sql NVARCHAR(200)
                SELECT @sql = 'ALTER TABLE [ResultatControles] DROP CONSTRAINT ' + d.name
                FROM sys.default_constraints d
                JOIN sys.columns c ON d.parent_object_id = c.object_id 
                                   AND d.parent_column_id = c.column_id
                WHERE d.parent_object_id = OBJECT_ID('ResultatControles')
                  AND c.name = 'Defaut1'
                IF @sql IS NOT NULL EXEC(@sql);
            ");

            // ✅ Supprimer DEFAULT sur Defaut2
            migrationBuilder.Sql(@"
                DECLARE @sql NVARCHAR(200)
                SELECT @sql = 'ALTER TABLE [ResultatControles] DROP CONSTRAINT ' + d.name
                FROM sys.default_constraints d
                JOIN sys.columns c ON d.parent_object_id = c.object_id 
                                   AND d.parent_column_id = c.column_id
                WHERE d.parent_object_id = OBJECT_ID('ResultatControles')
                  AND c.name = 'Defaut2'
                IF @sql IS NOT NULL EXEC(@sql);
            ");

            // ══════════════════════════════════════════
            // 5. SUPPRIMER COLONNE UtilisateurId1
            // ══════════════════════════════════════════

            migrationBuilder.Sql(@"
                DECLARE @sql NVARCHAR(200)
                SELECT @sql = 'ALTER TABLE [ResultatControles] DROP CONSTRAINT ' + d.name
                FROM sys.default_constraints d
                JOIN sys.columns c ON d.parent_object_id = c.object_id 
                                   AND d.parent_column_id = c.column_id
                WHERE d.parent_object_id = OBJECT_ID('ResultatControles')
                  AND c.name = 'UtilisateurId1'
                IF @sql IS NOT NULL EXEC(@sql);
            ");

            migrationBuilder.Sql(@"
                IF EXISTS (SELECT 1 FROM sys.columns 
                           WHERE name = 'UtilisateurId1'
                           AND object_id = OBJECT_ID('ResultatControles'))
                    ALTER TABLE [ResultatControles] DROP COLUMN [UtilisateurId1];
            ");

            // ══════════════════════════════════════════
            // 6. MODIFIER LES COLONNES
            // ══════════════════════════════════════════

            migrationBuilder.Sql(@"ALTER TABLE [ResultatControles] ALTER COLUMN [Id] NVARCHAR(20) NOT NULL;");
            migrationBuilder.Sql(@"ALTER TABLE [ResultatControles] ALTER COLUMN [CodeArticle] NVARCHAR(50) NOT NULL;");
            migrationBuilder.Sql(@"ALTER TABLE [ResultatControles] ALTER COLUMN [CodeMachine] NVARCHAR(50) NOT NULL;");
            migrationBuilder.Sql(@"ALTER TABLE [ResultatControles] ALTER COLUMN [NumOF] NVARCHAR(50) NOT NULL;");
            migrationBuilder.Sql(@"ALTER TABLE [ResultatControles] ALTER COLUMN [StatutLot] NVARCHAR(20) NOT NULL;");
            migrationBuilder.Sql(@"ALTER TABLE [ResultatControles] ALTER COLUMN [SolutionGlobale] NVARCHAR(500) NULL;");
            migrationBuilder.Sql(@"ALTER TABLE [ResultatControles] ALTER COLUMN [Defaut1] NVARCHAR(200) NULL;");
            migrationBuilder.Sql(@"ALTER TABLE [ResultatControles] ALTER COLUMN [Defaut2] NVARCHAR(200) NULL;");

            // ══════════════════════════════════════════
            // 7. RECRÉER LA CLÉ PRIMAIRE
            // ══════════════════════════════════════════

            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT 1 FROM sys.key_constraints 
                               WHERE name = 'PK_ResultatControles'
                               AND parent_object_id = OBJECT_ID('ResultatControles'))
                    ALTER TABLE [ResultatControles] 
                    ADD CONSTRAINT [PK_ResultatControles] PRIMARY KEY ([Id]);
            ");

            // ══════════════════════════════════════════
            // 8. AJOUTER CADENCE DANS PRODUITS
            // ══════════════════════════════════════════

            migrationBuilder.Sql(@"
                IF NOT EXISTS (
                    SELECT 1 FROM sys.columns 
                    WHERE name = 'Cadence' 
                    AND object_id = OBJECT_ID('Produits')
                )
                ALTER TABLE [Produits] ADD [Cadence] INT NOT NULL DEFAULT 1;
            ");

            // ══════════════════════════════════════════
            // 9. RECRÉER FK UTILISATEURS SEULEMENT
            // ══════════════════════════════════════════

            migrationBuilder.Sql(@"
                IF NOT EXISTS (
                    SELECT 1 FROM sys.foreign_keys 
                    WHERE name = 'FK_ResultatControles_Utilisateurs_UtilisateurId'
                )
                ALTER TABLE [ResultatControles]
                ADD CONSTRAINT [FK_ResultatControles_Utilisateurs_UtilisateurId]
                FOREIGN KEY ([UtilisateurId]) 
                REFERENCES [Utilisateurs]([Id]) 
                ON DELETE CASCADE;
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                IF EXISTS (SELECT 1 FROM sys.foreign_keys 
                           WHERE name = 'FK_ResultatControles_Utilisateurs_UtilisateurId')
                    ALTER TABLE [ResultatControles] 
                    DROP CONSTRAINT [FK_ResultatControles_Utilisateurs_UtilisateurId];
            ");

            migrationBuilder.Sql(@"
                IF EXISTS (SELECT 1 FROM sys.columns 
                           WHERE name = 'Cadence' 
                           AND object_id = OBJECT_ID('Produits'))
                    ALTER TABLE [Produits] DROP COLUMN [Cadence];
            ");
        }
    }
}