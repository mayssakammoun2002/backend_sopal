namespace Examen.ApplicationCore.Domain
{
    public class Seuil
    {
        public int Id { get; set; }
        public string CodeMachine { get; set; } = string.Empty;
        public string? CodeArticle { get; set; }

        public decimal SeuilPourcentage { get; set; }
        public bool EstActif { get; set; } = true;

        public DateTime DateCreation { get; set; } = DateTime.UtcNow;
        public DateTime? DateModification { get; set; }
        public int? CreePar { get; set; }

        // Relations (pour EF Core)
        public int? TypeDefaut1Id { get; set; }

        public Machine? Machine { get; set; }
        public Produit? Produit { get; set; }
        public TypeDefaut? TypeDefaut1 { get; set; }

        public ICollection<Alerte> Alertes { get; set; } = new List<Alerte>();
    }
}