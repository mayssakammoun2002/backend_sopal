using System.Collections.Generic;

namespace Examen.ApplicationCore.Entities
{
    public class TypeFonction
    {
        public int Id { get; set; }
        public string Nom { get; set; } = string.Empty;   // "CRUD Standard", "Lecture seule"
        public string? Description { get; set; }

        // Droits disponibles pour ce type (template)
        public bool PeutLire { get; set; }
        public bool PeutCreer { get; set; }
        public bool PeutModifier { get; set; }
        public bool PeutSupprimer { get; set; }

        public ICollection<Menu> Menus { get; set; } = new List<Menu>();
    }
}