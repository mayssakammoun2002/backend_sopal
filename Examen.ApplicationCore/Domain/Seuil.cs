using System;
using System.Collections.Generic;

namespace Examen.ApplicationCore.Domain
{
    public class Seuil
    {
        public int Id { get; set; }
        public string CodeMachine { get; set; } = string.Empty;
        public string? CodeArticle { get; set; }
        public int? TypeDefaut1Id { get; set; }
        public decimal SeuilPourcentage { get; set; } // ex: 5.00 pour 5%
        public int? SeuilQuantite { get; set; } // Optionnel: nombre min de défauts
        public bool EstActif { get; set; } = true;
        public DateTime DateCreation { get; set; } = DateTime.UtcNow;
        public DateTime? DateModification { get; set; }
        public int? CreePar { get; set; }

        // Navigation
        public Machine? Machine { get; set; }
        public Produit? Produit { get; set; }
        public TypeDefaut? TypeDefaut1 { get; set; }
        public ICollection<Alerte> Alertes { get; set; } = new List<Alerte>();
    }
}