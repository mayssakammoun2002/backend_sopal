using System.Collections.Generic;

namespace Examen.ApplicationCore.Entities
{
    public class Menu
    {
        public int Id { get; set; }
        public string Nom { get; set; } = string.Empty;
        public string? Icone { get; set; }           // ex: "fas fa-cog" pour l'UI
        public string? Lien { get; set; }
        public bool EstFonction { get; set; }
        public bool Visible { get; set; }
        public int Rang { get; set; }

        // Arborescence
        public int? ParentId { get; set; }
        public Menu? Parent { get; set; }
        public ICollection<Menu> Enfants { get; set; } = new List<Menu>();

        // Rempli uniquement si EstFonction = true
        public int? TypeFonctionId { get; set; }
        public TypeFonction? TypeFonction { get; set; }

        // Navigation
        public ICollection<ProfilMenu> ProfilMenus { get; set; } = new List<ProfilMenu>();
        public ICollection<ProfilFonctionDroit> ProfilFonctionDroits { get; set; } = new List<ProfilFonctionDroit>();
    }
}