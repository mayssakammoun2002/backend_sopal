using System.Collections.Generic;
using Examen.ApplicationCore.Entities;
namespace Examen.ApplicationCore.Domain
{
    public class Profil
    {
        public int Id { get; set; }
        public string Nom { get; set; }
        public string? Description { get; set; }
        public bool EstAdmin { get; set; } = false; // ✅ nouveau
        public ICollection<ProfilMenu> ProfilMenus { get; set; } = new List<ProfilMenu>();
        public ICollection<ProfilFonctionDroit> ProfilFonctionDroits { get; set; } = new List<ProfilFonctionDroit>();
        public ICollection<Utilisateur> Utilisateurs { get; set; } = new List<Utilisateur>();
    }
}