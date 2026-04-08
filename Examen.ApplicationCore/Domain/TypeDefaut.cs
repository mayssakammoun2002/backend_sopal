using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Examen.ApplicationCore.Domain
{
    public class TypeDefaut
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Le nom du défaut est obligatoire")]
        [StringLength(100)]
        public string NomDefaut { get; set; } = string.Empty;

        [Required(ErrorMessage = "La description est obligatoire")]
        [StringLength(500)]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "L'image est obligatoire")]
        public string ImagePath { get; set; } = string.Empty;

        [Required(ErrorMessage = "La solution est obligatoire")]
        [StringLength(500)]
        public string Solution { get; set; } = string.Empty;

        [Range(0, 1000, ErrorMessage = "La fréquence doit être positive")]
        public int Frequence { get; set; }

        // Clé étrangère vers Produit
        [ForeignKey("Produit")]
        public string CodeArticle { get; set; } = string.Empty;

        public Produit? Produit { get; set; }
    }
}