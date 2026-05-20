using System;
using System.Collections.Generic;

namespace Examen.ApplicationCore.Domain
{
    public class Alerte
    {
        public int Id { get; set; }
        public int SeuilId { get; set; }

        public string CodeMachine { get; set; } = string.Empty;
        public string? CodeArticle { get; set; }

        public int? TypeDefaut1Id { get; set; }
        public int? TypeDefaut2Id { get; set; }

        public decimal TauxDetecte { get; set; }
        public int QuantiteDefauts { get; set; }
        public int QuantiteTotale { get; set; }

        public NiveauAlerte Niveau { get; set; } = NiveauAlerte.Avertissement;
        public StatutAlerte Statut { get; set; } = StatutAlerte.Nouvelle;

        public string? Message { get; set; }

        public DateTime DateAlerte { get; set; } = DateTime.UtcNow;
        public DateTime? DateResolution { get; set; }

        public int? ResolueParId { get; set; }
        public string? CommentaireResolution { get; set; }

        // Navigation
        public Seuil Seuil { get; set; } = null!;
        public Machine? Machine { get; set; }
        public TypeDefaut? TypeDefaut1 { get; set; }
        public TypeDefaut? TypeDefaut2 { get; set; }
        public Utilisateur? ResolusePar { get; set; }
        public string? MachineId { get; set; }   // ou int ? selon votre modèle
        public ICollection<HistoriqueNotification> Notifications { get; set; }
            = new List<HistoriqueNotification>();
    }

    public enum NiveauAlerte { Avertissement = 1, Critique = 2, Urgence = 3 }
    public enum StatutAlerte { Nouvelle = 0, EnCours = 1, Resolue = 2, Ignoree = 3 }
}