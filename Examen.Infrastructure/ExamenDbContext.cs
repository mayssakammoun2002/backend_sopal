using Examen.ApplicationCore.Domain;
using Microsoft.EntityFrameworkCore;

namespace Examen.Infrastructure.Data
{
    public class ExamenDbContext : DbContext
    {
        public ExamenDbContext(DbContextOptions<ExamenDbContext> options)
            : base(options) { }

        // DbSets
        public DbSet<Machine> Machines { get; set; }
        public DbSet<Produit> Produits { get; set; }
        public DbSet<ResultatControle> ResultatControles { get; set; }
        public DbSet<TypeDefaut> TypeDefauts { get; set; }
        public DbSet<Utilisateur> Utilisateurs { get; set; }
        public DbSet<Seuil> Seuils { get; set; }
        public DbSet<Alerte> Alertes { get; set; }
        public DbSet<HistoriqueNotification> HistoriqueNotifications { get; set; }
        public DbSet<DestinataireNotification> DestinatairesNotification { get; set; }
        public DbSet<CommentaireAlerte> CommentairesAlerte { get; set; }
        public DbSet<PredictionDefaut> PredictionsDefauts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // ====================== CLÉS PRIMAIRES ======================
            modelBuilder.Entity<Machine>().HasKey(m => m.CodeMachine);
            modelBuilder.Entity<Produit>().HasKey(p => p.CodeArticle);
            modelBuilder.Entity<ResultatControle>().HasKey(r => r.Id);
            modelBuilder.Entity<TypeDefaut>().HasKey(t => t.Id);
            modelBuilder.Entity<Utilisateur>().HasKey(u => u.Id);

            // ====================== MACHINE ======================
            modelBuilder.Entity<Machine>()
                .Property(m => m.CodeMachine).HasMaxLength(20);
            modelBuilder.Entity<Machine>()
                .Property(m => m.NomMachine).IsRequired().HasMaxLength(50);

            // ====================== PRODUIT ======================
            modelBuilder.Entity<Produit>()
                .Property(p => p.CodeArticle).HasMaxLength(20);
            modelBuilder.Entity<Produit>()
                .Property(p => p.NomProduit).IsRequired().HasMaxLength(50);
            modelBuilder.Entity<Produit>()
                .Property(p => p.Designation).IsRequired().HasMaxLength(100);

            // ====================== RESULTAT CONTROLE ======================
            modelBuilder.Entity<ResultatControle>(entity =>
            {
                entity.Property(e => e.Id).HasMaxLength(36);
                entity.Property(e => e.CodeMachine).IsRequired().HasMaxLength(20);
                entity.Property(e => e.NumOF).IsRequired().HasMaxLength(50);
                entity.Property(e => e.StatutLot).HasMaxLength(20);
                entity.Property(e => e.SolutionGlobale).HasMaxLength(500);
                entity.Property(e => e.Defaut1).HasMaxLength(200);
                entity.Property(e => e.Defaut2).HasMaxLength(200);
                entity.Property(e => e.CodeArticle).IsRequired().HasMaxLength(20).HasColumnName("CodeArticle");

                entity.HasOne(r => r.Machine)
                      .WithMany(m => m.ResultatControles)
                      .HasForeignKey(r => r.CodeMachine)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(r => r.Utilisateur)
                      .WithMany(u => u.ResultatControles)
                      .HasForeignKey(r => r.UtilisateurId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(r => r.TypeDefaut1)
                      .WithMany()
                      .HasForeignKey(r => r.TypeDefaut1Id)
                      .IsRequired(false)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(r => r.TypeDefaut2)
                      .WithMany()
                      .HasForeignKey(r => r.TypeDefaut2Id)
                      .IsRequired(false)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.Ignore(r => r.Produit);
            });

            // ====================== TYPE DEFAUT ======================
            modelBuilder.Entity<TypeDefaut>()
                .Property(t => t.NomDefaut).IsRequired().HasMaxLength(100);
            modelBuilder.Entity<TypeDefaut>()
                .Property(t => t.Description).IsRequired().HasMaxLength(500);
            modelBuilder.Entity<TypeDefaut>()
                .Property(t => t.CauseProbable).IsRequired().HasMaxLength(500);
            modelBuilder.Entity<TypeDefaut>()
                .Property(t => t.Solution).IsRequired().HasMaxLength(500);

            // ====================== UTILISATEUR ======================
            modelBuilder.Entity<Utilisateur>()
                .Property(u => u.Email).IsRequired().HasMaxLength(100);
            modelBuilder.Entity<Utilisateur>()
                .Property(u => u.FirstName).IsRequired().HasMaxLength(50);
            modelBuilder.Entity<Utilisateur>()
                .Property(u => u.LastName).IsRequired().HasMaxLength(50);

            // ====================== SEUIL ======================
            modelBuilder.Entity<Seuil>(e =>
            {
                e.ToTable("Seuils");
                e.HasKey(x => x.Id);
                e.Property(x => x.CodeMachine).IsRequired().HasMaxLength(20);
                e.Property(x => x.CodeArticle).HasMaxLength(20);
                e.Property(x => x.SeuilPourcentage).HasColumnType("decimal(5,2)");

                e.HasOne(x => x.Machine)
                 .WithMany()
                 .HasForeignKey(x => x.CodeMachine)
                 .OnDelete(DeleteBehavior.Restrict);

                e.HasOne(x => x.Produit)
                 .WithMany()
                 .HasForeignKey(x => x.CodeArticle)
                 .IsRequired(false)
                 .OnDelete(DeleteBehavior.Restrict);

                e.HasOne(x => x.TypeDefaut1)
                 .WithMany()
                 .HasForeignKey(x => x.TypeDefaut1Id)
                 .IsRequired(false)
                 .OnDelete(DeleteBehavior.Restrict);

                e.HasMany(x => x.Alertes)
                 .WithOne(a => a.Seuil)
                 .HasForeignKey(a => a.SeuilId)
                 .OnDelete(DeleteBehavior.Restrict);
            });

            // ====================== ALERTE ======================
            modelBuilder.Entity<Alerte>(e =>
            {
                e.HasOne(x => x.Seuil)
                 .WithMany(s => s.Alertes)
                 .HasForeignKey(x => x.SeuilId)
                 .IsRequired(false)
                 .OnDelete(DeleteBehavior.SetNull);

                e.HasOne(x => x.ResoluePar)
                 .WithMany()
                 .HasForeignKey(x => x.ResolueParId)
                 .IsRequired(false)
                 .OnDelete(DeleteBehavior.Restrict);

                e.HasMany(x => x.Commentaires)
                 .WithOne(c => c.Alerte)
                 .HasForeignKey(c => c.AlerteId)
                 .OnDelete(DeleteBehavior.Cascade);

                e.HasMany(x => x.Notifications)
                 .WithOne(n => n.Alerte)
                 .HasForeignKey(n => n.AlerteId)
                 .OnDelete(DeleteBehavior.Cascade);

                e.HasIndex(x => x.DateAlerte);
                e.HasIndex(x => x.Statut);
                e.HasIndex(x => x.CodeMachine);
                e.HasIndex(x => x.SeuilId);
            });

            // ====================== COMMENTAIRE ALERTE ======================
            modelBuilder.Entity<CommentaireAlerte>(e =>
            {
                e.ToTable("CommentairesAlerte");
                e.HasKey(x => x.Id);
                e.Property(x => x.NomAuteur).IsRequired().HasMaxLength(100);
                e.Property(x => x.Contenu).IsRequired().HasMaxLength(2000);

                e.HasOne(x => x.Alerte)
                 .WithMany(a => a.Commentaires)
                 .HasForeignKey(x => x.AlerteId)
                 .OnDelete(DeleteBehavior.Cascade);
            });

            // ====================== HISTORIQUE NOTIFICATION ======================
            modelBuilder.Entity<HistoriqueNotification>(e =>
            {
                e.ToTable("HistoriqueNotifications");
                e.HasKey(x => x.Id);
                e.Property(x => x.Canal).HasConversion<byte>();
                e.Property(x => x.Statut).HasConversion<byte>();
                e.Property(x => x.Destinataire).IsRequired().HasMaxLength(255);
                e.Property(x => x.Sujet).HasMaxLength(255);
                e.Property(x => x.Corps).HasColumnType("nvarchar(max)");
                e.Property(x => x.ErreurMessage).HasMaxLength(500);

                e.HasOne(x => x.Alerte)
                 .WithMany(a => a.Notifications)
                 .HasForeignKey(x => x.AlerteId)
                 .OnDelete(DeleteBehavior.Cascade);

                e.HasIndex(x => x.AlerteId);
                e.HasIndex(x => x.Statut);
            });
            // ====================== DESTINATAIRE NOTIFICATION ======================
            modelBuilder.Entity<DestinataireNotification>(e =>
            {
                e.ToTable("DestinatairesNotification");
                e.HasKey(x => x.Id);
                e.Property(x => x.Canal).HasConversion<byte>();
                e.Property(x => x.NiveauMinimum).HasConversion<byte>();
                e.Property(x => x.Destinataire).IsRequired().HasMaxLength(255);
                e.Property(x => x.Role).HasMaxLength(50);

                e.HasOne(x => x.Utilisateur)
                 .WithMany()
                 .HasForeignKey(x => x.UtilisateurId)
                 .IsRequired(false)
                 .OnDelete(DeleteBehavior.Restrict);
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}