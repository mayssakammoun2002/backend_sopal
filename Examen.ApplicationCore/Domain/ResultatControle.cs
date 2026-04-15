using System.ComponentModel.DataAnnotations;
using Examen.ApplicationCore.Domain;

namespace Examen.ApplicationCore.Domain
{
    public class ResultatControle
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        public DateTime DateControle { get; set; } = DateTime.UtcNow;

        [Required]
        public string CodeMachine { get; set; } = null!;

        [Required]
        public string CodeArticle { get; set; } = null!;

        [Required]
        public int UtilisateurId { get; set; }

        [Required]
        public string NumOF { get; set; } = null!;

        [Range(1, int.MaxValue)]
        public int Quantite { get; set; }

        [Range(1, 1000)]
        public int Cadence { get; set; }

        [Range(1, 100)]
        public int NbEchantillons { get; set; }

        [RegularExpression(@"^(Conforme|Non Conforme)$")]
        public string StatutLot { get; set; } = "Conforme";

        public string? SolutionGlobale { get; set; }

        // === Défauts (maximum 2) ===
        public int? TypeDefaut1Id { get; set; }
        public int? TypeDefaut2Id { get; set; }

        public int NbDefautsTest1 { get; set; } = 0;
        public int NbDefautsTest2 { get; set; } = 0;

        [StringLength(200)]
        public string? Defaut1 { get; set; }

        [StringLength(200)]
        public string? Defaut2 { get; set; }

        // === Navigation Properties ===
        public Machine Machine { get; set; } = null!;
        public Produit Produit { get; set; } = null!;
        public Utilisateur Utilisateur { get; set; } = null!;
        public TypeDefaut? TypeDefaut1 { get; set; }
        public TypeDefaut? TypeDefaut2 { get; set; }
    }
}