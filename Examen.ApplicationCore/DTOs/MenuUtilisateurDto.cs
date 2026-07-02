using Examen.ApplicationCore.DTOs;

public class MenuUtilisateurDto
{
    public int Id { get; set; }
    public string Nom { get; set; } = string.Empty;
    public string? Icone { get; set; }
    public string? Lien { get; set; }
    public bool EstFonction { get; set; }
    public int Rang { get; set; }
    public DroitsDto? Droits { get; set; } // rempli uniquement si EstFonction
    public List<MenuUtilisateurDto> Enfants { get; set; } = new();
}