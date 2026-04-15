using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Examen.ApplicationCore.Domain
{
    public class Produit
    {
        [Key]
        [MaxLength(20)]
        [Required(ErrorMessage = "Le code article est obligatoire")]
        public string CodeArticle { get; set; }

        [Required(ErrorMessage = "Le nom du produit est obligatoire")]
        [StringLength(50, ErrorMessage = "Le nom ne doit pas dépasser 50 caractères")]
        public string NomProduit { get; set; }

        [Required(ErrorMessage = "La désignation est obligatoire")]
        [StringLength(100, MinimumLength = 3,
            ErrorMessage = "La désignation doit contenir entre 3 et 100 caractères")]
        public string Designation { get; set; }

        [Range(1, 1000, ErrorMessage = "La taille d'échantillonnage doit être entre 1 et 1000")]
        public int TailleEchantillonnage { get; set; }

        [Range(1, 10000, ErrorMessage = "La cadence doit être entre 1 et 10000")]
        public int Cadence { get; set; }

        // Relation : 1 Produit → plusieurs TypeDefaut
        public ICollection<TypeDefaut> TypeDefauts { get; set; } = new List<TypeDefaut>();
    }
}