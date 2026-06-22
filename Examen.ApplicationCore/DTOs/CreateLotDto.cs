using System.ComponentModel.DataAnnotations;

public class CreateLotDto
{
    [Required, MaxLength(50)]
    public string NumeroLot { get; set; } = string.Empty;

    [Required]
    public string MachineId { get; set; } = string.Empty;  // string ✅

    [Required]
    public string ProduitId { get; set; } = string.Empty;  // ✅ FIX : string pas int

    [Required]
    public int OperateurId { get; set; }

    [Required]
    public DateTime DateDebut { get; set; }

    public DateTime? DateFin { get; set; }

    [Required, Range(1, int.MaxValue)]
    public int QuantitePrevue { get; set; }

    public int? QuantiteProduite { get; set; }

    public string Statut { get; set; } = "En cours";

    [MaxLength(500)]
    public string? Commentaire { get; set; }
}