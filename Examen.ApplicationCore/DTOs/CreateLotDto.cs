using System.ComponentModel.DataAnnotations;

public class CreateLotDto
{
    [Required, MaxLength(50)] public string NumeroLot { get; set; } = string.Empty;
    [Required] public string MachineId { get; set; } = string.Empty;  // ✅ string
    [Required] public int ProduitId { get; set; }
    [Required] public int OperateurId { get; set; }
    [Required] public DateTime DateDebut { get; set; }
    [Required, Range(1, int.MaxValue)] public int QuantitePrevue { get; set; }
    [MaxLength(500)] public string? Commentaire { get; set; }
}