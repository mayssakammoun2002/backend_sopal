using Examen.ApplicationCore.Domain;
using System.ComponentModel.DataAnnotations;

public class Machine
{
    [Key]

    [Required]
    [RegularExpression(@"^[A-Za-z0-9]+$")]
    public string CodeMachine { get; set; }

    [Required]
    [StringLength(50, MinimumLength = 2)]
    public string NomMachine { get; set; }

    public bool Actif { get; set; } = true;

    public ICollection<ResultatControle> ResultatControles { get; set; }
    [MaxLength(50)]
    public string Statut { get; set; } = "Active";

    public ICollection<Lot> Lots { get; set; } = new List<Lot>();
}