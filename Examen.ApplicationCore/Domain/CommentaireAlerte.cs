using Examen.ApplicationCore.Domain;

public class CommentaireAlerte
{
    public int Id { get; set; }
    public int AlerteId { get; set; }
    public Alerte? Alerte { get; set; }
    public int AuteurId { get; set; }
    public string NomAuteur { get; set; } = string.Empty;
    public string Contenu { get; set; } = string.Empty;
    public DateTime DateCreation { get; set; } = DateTime.UtcNow;
}