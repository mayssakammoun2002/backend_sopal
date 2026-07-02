using Examen.ApplicationCore.Domain;

namespace Examen.ApplicationCore.Entities
{
    // Table de jonction : droits d'un profil sur une fonction précise
    public class ProfilFonctionDroit
    {
        public int ProfilId { get; set; }
        public Profil Profil { get; set; } = null!;

        public int MenuId { get; set; }   // Menu où EstFonction = true
        public Menu Menu { get; set; } = null!;

        public bool Lecture { get; set; }
        public bool Creation { get; set; }
        public bool Modification { get; set; }
        public bool Suppression { get; set; }
    }
}