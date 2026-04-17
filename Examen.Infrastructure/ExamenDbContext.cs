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

        public DbSet<Machine> Machines { get; set; }
        public DbSet<Produit> Produits { get; set; }
        public DbSet<ResultatControle> ResultatControles { get; set; }
        public DbSet<TypeDefaut> TypeDefauts { get; set; }
        public DbSet<Utilisateur> Utilisateurs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // ====================== CLÉS PRIMAIRES ======================
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

            // ====================== MACHINE ======================
            modelBuilder.Entity<Machine>()
                .Property(m => m.CodeMachine)
                .HasMaxLength(20);

            modelBuilder.Entity<Machine>()
                .Property(m => m.NomMachine)
                .IsRequired()
                .HasMaxLength(50);

            // ====================== PRODUIT ======================
            modelBuilder.Entity<Produit>()
                .Property(p => p.CodeArticle)
                .HasMaxLength(20);

            modelBuilder.Entity<Produit>()
                .Property(p => p.NomProduit)
                .IsRequired()
                .HasMaxLength(50);

            modelBuilder.Entity<Produit>()
                .Property(p => p.Designation)
                .IsRequired()
                .HasMaxLength(100);

            // ====================== RESULTAT CONTROLE ======================
            modelBuilder.Entity<ResultatControle>(entity =>
            {
                entity.Property(e => e.Id)
                      .HasMaxLength(36);

                entity.Property(e => e.CodeMachine)
                      .IsRequired()
                      .HasMaxLength(20);


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

                // --- Relation avec Machine ---
                entity.HasOne(r => r.Machine)
                      .WithMany(m => m.ResultatControles)
                      .HasForeignKey(r => r.CodeMachine)
                      .OnDelete(DeleteBehavior.Restrict);
                // --- Relation avec Produit --- REMPLACER PAR :
                entity.Ignore(r => r.Produit);
                entity.Property(e => e.CodeArticle)
                      .IsRequired()
                      .HasMaxLength(20)
                      .HasColumnName("CodeArticle");
                // --- Relation avec Utilisateur ---
                entity.HasOne(r => r.Utilisateur)
                      .WithMany(u => u.ResultatControles)
                      .HasForeignKey(r => r.UtilisateurId)
                      .OnDelete(DeleteBehavior.Restrict);

                // --- Relation avec TypeDefaut1 (pas de navigation inverse) ---
                entity.HasOne(r => r.TypeDefaut1)
                      .WithMany()
                      .HasForeignKey(r => r.TypeDefaut1Id)
                      .IsRequired(false)
                      .OnDelete(DeleteBehavior.Restrict);

                // --- Relation avec TypeDefaut2 (pas de navigation inverse) ---
                entity.HasOne(r => r.TypeDefaut2)
                      .WithMany()
                      .HasForeignKey(r => r.TypeDefaut2Id)
                      .IsRequired(false)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // ====================== TYPE DEFAUT ======================
            modelBuilder.Entity<TypeDefaut>()
                .Property(t => t.NomDefaut)
                .IsRequired()
                .HasMaxLength(100);

            modelBuilder.Entity<TypeDefaut>()
                .Property(t => t.Description)
                .IsRequired()
                .HasMaxLength(500);

            modelBuilder.Entity<TypeDefaut>()
                .Property(t => t.CauseProbable)
                .IsRequired()
                .HasMaxLength(500);

            modelBuilder.Entity<TypeDefaut>()
                .Property(t => t.Solution)
                .IsRequired()
                .HasMaxLength(500);

            // ====================== UTILISATEUR ======================
            modelBuilder.Entity<Utilisateur>()
                .Property(u => u.Email)
                .IsRequired()
                .HasMaxLength(100);

            modelBuilder.Entity<Utilisateur>()
                .Property(u => u.FirstName)
                .IsRequired()
                .HasMaxLength(50);

            modelBuilder.Entity<Utilisateur>()
                .Property(u => u.LastName)
                .IsRequired()
                .HasMaxLength(50);

            base.OnModelCreating(modelBuilder);
        }
    }
}