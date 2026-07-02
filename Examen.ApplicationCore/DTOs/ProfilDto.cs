using System.Collections.Generic;

namespace Examen.ApplicationCore.DTOs
{
    public class ProfilDto
    {
        public int Id { get; set; }
        public string Nom { get; set; } = string.Empty;
        public string? Description { get; set; }
        public List<ProfilMenuDto> Menus { get; set; } = new();
        public List<ProfilFonctionDroitDto> FonctionDroits { get; set; } = new();
        public int NbUtilisateurs { get; set; }
    }
}