using System.ComponentModel.DataAnnotations;

namespace Examen.ApplicationCore.DTOs
{
    public class MachineCreateDto
    {
        [Required]
        [RegularExpression(@"^[A-Za-z0-9\-]+$", ErrorMessage = "Le code machine ne peut contenir que des lettres, chiffres et tirets")]
        [StringLength(20)]
        public string CodeMachine { get; set; } = string.Empty;

        [Required]
        [StringLength(50, MinimumLength = 2)]
        public string NomMachine { get; set; } = string.Empty;

        public bool Actif { get; set; } = true;
    }

    public class MachineUpdateDto
    {
        [Required]
        [StringLength(50, MinimumLength = 2)]
        public string NomMachine { get; set; } = string.Empty;

        public bool Actif { get; set; } = true;
    }
}