using System;
using System.ComponentModel.DataAnnotations;

namespace Examen.ApplicationCore.Domain
{
    public class ResultatControle
    {
        [Key]
        public string Id { get; set; } = string.Empty;

        [Range(1, 100)]
        public int NbEchantillons { get; set; }

        public DateTime DateControle { get; set; } = DateTime.UtcNow;

        [RegularExpression(@"^(Conforme|Non Conforme)$")]
        public string StatutLot { get; set; } = string.Empty;

        // Simple string - pas de relation
        [Required]
        public string CodeMachine { get; set; } = string.Empty;

        // Simple string - pas de relation (CodeArticle libre)
        [Required]
        public string CodeArticle { get; set; } = string.Empty;

        // Simple int - pas de relation
        [Required]
        public int UtilisateurId { get; set; }

        [Required]
        public string NumOF { get; set; } = string.Empty;

        [Range(1, int.MaxValue)]
        public int Quantite { get; set; }

        [Range(1, 1000)]
        public int Cadence { get; set; }

        public string? SolutionGlobale { get; set; }

        public int NbDefautsTest1 { get; set; }
        public int NbDefautsTest2 { get; set; }

        public string? Defaut1 { get; set; }
        public string? Defaut2 { get; set; }

        // ❌ SUPPRIMÉ : public string? Machine { get; internal set; }
    }
}