using Examen.ApplicationCore.Domain;
using System.ComponentModel.DataAnnotations;

public class Machine
{
    [Key]
    [Required]
    [RegularExpression(@"^[A-Za-z0-9]+$", ErrorMessage = "Code invalide")]
    public string CodeMachine { get; set; } = null!;

    [Required(ErrorMessage = "Le nom machine est obligatoire")]
    [StringLength(50, MinimumLength = 2, ErrorMessage = "Le nom doit contenir entre 2 et 50 caractères")]
    public string NomMachine { get; set; } = null!;

    public bool Actif { get; set; } = true;

    public ICollection<ResultatControle> ResultatControles { get; set; } = new List<ResultatControle>();
}