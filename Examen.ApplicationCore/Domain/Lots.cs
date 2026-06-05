using Examen.ApplicationCore.Domain;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Lot
{
    public int Id { get; set; }

    [Required, MaxLength(50)]
    public string NumeroLot { get; set; } = string.Empty;

    [ForeignKey(nameof(Machine))]
    public string MachineId { get; set; } = string.Empty;
    public Machine Machine { get; set; } = null!;

    // ✅ Fixed: string instead of int
    [ForeignKey(nameof(Produit))]
    public string ProduitId { get; set; } = string.Empty;   // ← changed to string

    public Produit Produit { get; set; } = null!;

    [ForeignKey(nameof(Operateur))]
    public int OperateurId { get; set; }
    public Utilisateur Operateur { get; set; } = null!;

    public DateTime DateDebut { get; set; }
    public DateTime? DateFin { get; set; }
    public int QuantitePrevue { get; set; }
    public int? QuantiteProduite { get; set; }

    [MaxLength(20)]
    public string Statut { get; set; } = StatutLot.EnCours;

    [MaxLength(500)]
    public string? Commentaire { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    public ICollection<ResultatControle> ResultatsControle { get; set; } = new List<ResultatControle>();
    public ICollection<Alerte> Alertes { get; set; } = new List<Alerte>();
}
public static class StatutLot
{
    public const string EnCours = "En cours";
    public const string Termine = "Terminé";
    public const string Suspendu = "Suspendu";
    public const string Rejete = "Rejeté";

    private static readonly HashSet<string> _valides =
        new HashSet<string> { EnCours, Termine, Suspendu, Rejete };

    public static bool IsValid(string statut) =>
        !string.IsNullOrWhiteSpace(statut) && _valides.Contains(statut);
}