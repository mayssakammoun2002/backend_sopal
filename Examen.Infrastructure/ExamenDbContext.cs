using Examen.ApplicationCore.Domain;
using Microsoft.EntityFrameworkCore;

namespace Examen.Infrastructure.Data
{
    public class ExamenDbContext : DbContext
    {
        public ExamenDbContext(DbContextOptions<ExamenDbContext> options)
            : base(options)
        {
        }

        // Tables / DbSets
        public DbSet<Machine> Machines { get; set; }
        public DbSet<Produit> Produits { get; set; }
        public DbSet<ResultatControle> ResultatControles { get; set; }
        public DbSet<TypeDefaut> TypeDefauts { get; set; }
        public DbSet<Utilisateur> Utilisateurs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configuration des clés primaires
            modelBuilder.Entity<Machine>()
                .HasKey(m => m.CodeMachine);

            modelBuilder.Entity<Produit>()
                .HasKey(p => p.CodeArticle);

            modelBuilder.Entity<ResultatControle>()
                .HasKey(r => r.Id);

            modelBuilder.Entity<TypeDefaut>()
                .HasKey(t => t.Id);

            modelBuilder.Entity<Utilisateur>()
                .HasKey(u => u.Id);

            // ====================== LONGUEURS DES COLONNES ======================
            modelBuilder.Entity<Machine>()
                .Property(m => m.CodeMachine)
                .HasMaxLength(20);

            modelBuilder.Entity<Produit>()
                .Property(p => p.CodeArticle)
                .HasMaxLength(20);

            modelBuilder.Entity<Produit>()
                .Property(p => p.NomProduit)
                .HasMaxLength(50);

            modelBuilder.Entity<Produit>()
                .Property(p => p.Designation)
                .HasMaxLength(100);

            // ====================== CONFIGURATION DE RESULTATCONTROLE ======================
            // Configuration simplifiée (sans relations de navigation)
            modelBuilder.Entity<ResultatControle>(entity =>
            {
                entity.Property(e => e.Id)
                      .HasMaxLength(20);

                entity.Property(e => e.CodeMachine)
                      .IsRequired()
                      .HasMaxLength(50);

                entity.Property(e => e.CodeArticle)
                      .IsRequired()
                      .HasMaxLength(50);

                entity.Property(e => e.NumOF)
                      .IsRequired()
                      .HasMaxLength(50);

                entity.Property(e => e.StatutLot)
                      .HasMaxLength(20);

                entity.Property(e => e.SolutionGlobale)
                      .HasMaxLength(500);

                entity.Property(e => e.Defaut1)
                      .HasMaxLength(200);

                entity.Property(e => e.Defaut2)
                      .HasMaxLength(200);

                // IMPORTANT : Pas de HasOne() car il n'y a plus de propriétés Machine et Utilisateur
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}