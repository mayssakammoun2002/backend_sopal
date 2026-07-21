using System.ComponentModel.DataAnnotations;

namespace Examen.ApplicationCore.DTOs
{
    public class MatriculeDTO
    {
        [Required(ErrorMessage = "Le matricule est obligatoire")]
        [StringLength(20)]
        public string Matricule { get; set; } = string.Empty;
    }
}