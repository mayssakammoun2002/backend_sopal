public class PredictionDefautDto
{
    public int Id { get; set; }
    public string ResultatControleId { get; set; } = string.Empty;
    public bool EstDefectueux { get; set; }
    public float Probabilite { get; set; }
    public string NiveauRisque { get; set; } = string.Empty;
    public DateTime DatePrediction { get; set; }
    public string ModelVersion { get; set; } = string.Empty;

    // ← Nom du défaut au lieu de l'ID
    public string? NomDefaut { get; set; }
    public string? DescriptionDefaut { get; set; }
}