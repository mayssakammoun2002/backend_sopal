public class ProfilFonctionDroitDto
{
    public int MenuId { get; set; }
    public string NomFonction { get; set; } = string.Empty;
    public bool Lecture { get; set; }
    public bool Creation { get; set; }
    public bool Modification { get; set; }
    public bool Suppression { get; set; }
}
