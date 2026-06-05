using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Examen.ApplicationCore.Domain
{
    public class PredictionDefaut
    {
        [Key]
        public int Id { get; set; }

        // ── Lien vers le contrôle ────────────────────────────────────────────
        [Required]
        public string ResultatControleId { get; set; } = string.Empty;

        // ── Résultat de la prédiction ────────────────────────────────────────
        public bool EstDefectueux { get; set; }

        [Column(TypeName = "decimal(5,4)")]
        public float Probabilite { get; set; }

        [MaxLength(20)]
        public string NiveauRisque { get; set; } = string.Empty;  // Faible / Moyen / Élevé

        public int? TypeDefautPreditId { get; set; }

        // ── Traçabilité ──────────────────────────────────────────────────────
        public DateTime DatePrediction { get; set; } = DateTime.UtcNow;

        [MaxLength(20)]
        public string ModelVersion { get; set; } = string.Empty;

        public int? LatenceMs { get; set; }   // durée de l'appel FastAPI

        // Stocke les features envoyées (JSON) — utile pour rejouer une prédiction
        [Column(TypeName = "nvarchar(max)")]
        public string? FeaturesJson { get; set; }

        // Explication SHAP sérialisée (JSON) — optionnel
        [Column(TypeName = "nvarchar(max)")]
        public string? ShapExplicationJson { get; set; }
        // Ajoute cette propriété de navigation
        [ForeignKey("TypeDefautPreditId")]
        public TypeDefaut? TypeDefautPredit { get; set; }
    }
}