using Microsoft.EntityFrameworkCore;
using Examen.ApplicationCore.Entities;
using Examen.ApplicationCore.Domain;

namespace Examen.Infrastructure.Data.Configurations
{
    public class AccesConfigurations
    {
        public static void Configure(ModelBuilder modelBuilder)
        {
            // Menu : auto-référencé pour l'arborescence
            modelBuilder.Entity<Menu>()
                .HasOne(m => m.Parent)
                .WithMany(m => m.Enfants)
                .HasForeignKey(m => m.ParentId)
                .OnDelete(DeleteBehavior.Restrict);

            // Menu -> TypeFonction (optionnel, seulement si EstFonction = true)
            modelBuilder.Entity<Menu>()
                .HasOne(m => m.TypeFonction)
                .WithMany(t => t.Menus)
                .HasForeignKey(m => m.TypeFonctionId)
                .OnDelete(DeleteBehavior.Restrict);

            // ProfilMenu : clé composite + relations
            modelBuilder.Entity<ProfilMenu>()
                .HasKey(pm => new { pm.ProfilId, pm.MenuId });

            modelBuilder.Entity<ProfilMenu>()
                .HasOne(pm => pm.Profil)
                .WithMany(p => p.ProfilMenus)
                .HasForeignKey(pm => pm.ProfilId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ProfilMenu>()
                .HasOne(pm => pm.Menu)
                .WithMany(m => m.ProfilMenus)
                .HasForeignKey(pm => pm.MenuId)
                .OnDelete(DeleteBehavior.Cascade);

            // ProfilFonctionDroit : clé composite + relations
            modelBuilder.Entity<ProfilFonctionDroit>()
                .HasKey(pfd => new { pfd.ProfilId, pfd.MenuId });

            modelBuilder.Entity<ProfilFonctionDroit>()
                .HasOne(pfd => pfd.Profil)
                .WithMany(p => p.ProfilFonctionDroits)
                .HasForeignKey(pfd => pfd.ProfilId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ProfilFonctionDroit>()
                .HasOne(pfd => pfd.Menu)
                .WithMany(m => m.ProfilFonctionDroits)
                .HasForeignKey(pfd => pfd.MenuId)
                .OnDelete(DeleteBehavior.Cascade);

            // Utilisateur -> Profil
            modelBuilder.Entity<Utilisateur>()
                .HasOne(u => u.Profil)
                .WithMany(p => p.Utilisateurs)
                .HasForeignKey(u => u.ProfilId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}