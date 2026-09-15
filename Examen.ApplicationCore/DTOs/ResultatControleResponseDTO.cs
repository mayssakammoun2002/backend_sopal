using System;
using System.Collections.Generic;

namespace Examen.ApplicationCore.DTOs
{
    public class ResultatControleResponseDTO
    {
        public string Id { get; set; } = "";

        public DateTime DateControle { get; set; }

        public string CodeMachine { get; set; } = "";

        public string NomMachine { get; set; } = "";

        public string CodeArticle { get; set; } = "";

        public string NomProduit { get; set; } = "";

        public string NumOF { get; set; } = "";

        public string? NumLotMatiere { get; set; }

        public string? NumConteneur { get; set; }

        public int Quantite { get; set; }

        public int Cadence { get; set; }

        public int NbEchantillons { get; set; }

        public string StatutLot { get; set; } = "Conforme";

        public int NbDefautsTest1 { get; set; }

        public int NbDefautsTest2 { get; set; }

        public string? SolutionGlobale { get; set; }

        public string? Defaut1 { get; set; }

        public string? Defaut2 { get; set; }

        /*
         * Utilisateur ayant saisi le contrôle (traçabilité).
         */
        public int UtilisateurId { get; set; }

        public string SaisiPar { get; set; } = "Inconnu";

        /*
         * Contrôleurs réellement sélectionnés.
         */
        public string Controleur { get; set; } = "—";

        public List<int> ControleurIds { get; set; } = new List<int>();
    }
}