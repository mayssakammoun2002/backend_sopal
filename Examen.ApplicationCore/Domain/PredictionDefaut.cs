using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Examen.ApplicationCore.Domain
{
    public class PredictionDefaut
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string ResultatControleId { get; set; } = string.Empty;

        public bool EstDefectueux { get; set; }

        [Column(TypeName = "decimal(5,4)")]
        public float Probabilite { get; set; }

        [MaxLength(20)]
        public string NiveauRisque { get; set; } = string.Empty;  

        public int? TypeDefautPreditId { get; set; }

        public DateTime DatePrediction { get; set; } = DateTime.UtcNow;

        [MaxLength(20)]
        public string ModelVersion { get; set; } = string.Empty;

        public int? LatenceMs { get; set; }  
        [Column(TypeName = "nvarchar(max)")]
        public string? FeaturesJson { get; set; }

        [Column(TypeName = "nvarchar(max)")]
        public string? ShapExplicationJson { get; set; }
        [ForeignKey("TypeDefautPreditId")]
        public TypeDefaut? TypeDefautPredit { get; set; }
    }
}