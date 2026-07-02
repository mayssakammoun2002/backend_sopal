using Examen.ApplicationCore.Domain;

namespace Examen.ApplicationCore.Entities
{
    public class ProfilMenu
    {
        public int ProfilId { get; set; }
        public Profil Profil { get; set; } = null!;

        public int MenuId { get; set; }
        public Menu Menu { get; set; } = null!;

        public bool Visible { get; set; }
    }
}