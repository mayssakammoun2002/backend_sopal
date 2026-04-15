using System;

namespace Examen.ApplicationCore.DTOs
{
    public class ResultatControleResponseDTO
    {
        public string Id { get; set; } = string.Empty;
        public DateTime DateControle { get; set; }

        // Machine
        public string CodeMachine { get; set; } = string.Empty;
        public string NomMachine { get; set; } = "N/A";

        // Produit
        public string CodeArticle { get; set; } = string.Empty;
        public string NomProduit { get; set; } = "N/A";

        // Informations de production
        public string NumOF { get; set; } = string.Empty;
        public int Quantite { get; set; }
        public int Cadence { get; set; }

        // Résultats du contrôle
        public int NbEchantillons { get; set; }
        public string StatutLot { get; set; } = string.Empty;

        public int NbDefautsTest1 { get; set; }
        public int NbDefautsTest2 { get; set; }

        public string? SolutionGlobale { get; set; }

        // Contrôleur
        public int UtilisateurId { get; set; }
        public string Controleur { get; set; } = "Inconnu";

        // Défauts
        public string? Defaut1 { get; set; }
        public string? Defaut2 { get; set; }
    }
}