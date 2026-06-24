using System;
using System.ComponentModel.DataAnnotations;

public class ResultatControleDTO
{
    [Required(ErrorMessage = "Code Machine est obligatoire")]
    public string CodeMachine { get; set; } = string.Empty;

    [Required(ErrorMessage = "Code Article est obligatoire")]
    public string CodeArticle { get; set; } = string.Empty;

    [Required(ErrorMessage = "Num OF est obligatoire")]
    public string NumOF { get; set; } = string.Empty;

    // ✅ CORRECTION : int? au lieu de int
    // [Required] sur un int normal échoue toujours car int = 0 par défaut
    // Avec int?, ASP.NET Core peut détecter null et déclencher la validation correctement
    [Required(ErrorMessage = "Utilisateur est obligatoire")]
    public int? UtilisateurId { get; set; }

    [Required(ErrorMessage = "Quantité produite est obligatoire")]
    [Range(1, int.MaxValue, ErrorMessage = "Quantité doit être > 0")]
    public int Quantite { get; set; }

    [Required(ErrorMessage = "Cadence est obligatoire")]
    [Range(1, 10000, ErrorMessage = "Cadence doit être entre 1 et 10000")]
    public int Cadence { get; set; }
    public int NbEchantillons { get; set; } = 3;
    public int NbDefautsTest1 { get; set; } = 0;
    public int NbDefautsTest2 { get; set; } = 0;
    public string? SolutionGlobale { get; set; }
    public string? Defaut1 { get; set; }
    public string? Defaut2 { get; set; }
    public DateTime? DateControle { get; set; }
    public string? NumLotMatiere { get; internal set; }
}