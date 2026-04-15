using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Examen.ApplicationCore.Domain
{
    public class Machine
    {
        [Key]
        [StringLength(8, MinimumLength = 8, ErrorMessage = "Le code machine doit contenir exactement 8 caractères")]
        [RegularExpression(@"^[A-Za-z0-9]{8}$", ErrorMessage = "Le code doit être alphanumérique (8 caractères)")]
        public string CodeMachine { get; set; } = null!;

        [Required(ErrorMessage = "Le nom machine est obligatoire")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Le nom doit contenir entre 2 et 50 caractères")]
        public string NomMachine { get; set; } = null!;

        public bool Actif { get; set; } = true;


        public ICollection<ResultatControle> ResultatControles { get; set; } = new List<ResultatControle>();
    }
}