using System.ComponentModel.DataAnnotations;

namespace Examen.ApplicationCore.Domain
{
    public class TypeDefaut
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string NomDefaut { get; set; } = string.Empty;

        [Required]
        [StringLength(500)]
        public string Description { get; set; } = string.Empty;

        [Required]
        [StringLength(500)]
        public string CauseProbable { get; set; } = string.Empty;

        [Required]
        [StringLength(500)]
        public string Solution { get; set; } = string.Empty;

        [Range(0, 1000)]
        public int Frequence { get; set; }

        public string ImagePath { get; set; } = string.Empty;


    }
}