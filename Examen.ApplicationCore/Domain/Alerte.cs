using System;
using System.Collections.Generic;

namespace Examen.ApplicationCore.Domain
{
    public class Alerte
    {
        public int Id { get; set; }

        // Foreign Keys
        public int SeuilId { get; set; }
        // public int MachineId { get; set; }

        public int? ResolueParId { get; set; }

        // Properties
        public string CodeMachine { get; set; } = string.Empty;
        public string? CodeArticle { get; set; }

        // Navigation Properties
        public Seuil Seuil { get; set; } = null!;
        public Machine Machine { get; set; } = null!;
        public Utilisateur? ResoluePar { get; set; }

        public ICollection<CommentaireAlerte> Commentaires { get; set; } = new List<CommentaireAlerte>();
        public ICollection<HistoriqueNotification> Notifications { get; set; } = new List<HistoriqueNotification>();

        // Data Properties
        public decimal TauxDetecte { get; set; }
        public int QuantiteDefauts { get; set; }
        public int QuantiteTotale { get; set; }

        public NiveauAlerte Niveau { get; set; }
        public StatutAlerte Statut { get; set; } = StatutAlerte.Nouvelle;

        public string Message { get; set; } = string.Empty;
        public string? CommentaireResolution { get; set; }

        // Timestamps
        public DateTime DateAlerte { get; set; } = DateTime.UtcNow;
        public DateTime? DateResolution { get; set; }

        // Business Methods
        public void Resoudre(int utilisateurId, string? commentaire = null)
        {
            Statut = StatutAlerte.Resolue;
            DateResolution = DateTime.UtcNow;
            ResolueParId = utilisateurId;
            CommentaireResolution = commentaire;
        }

        public void MarquerEnCours() => Statut = StatutAlerte.EnCours;
        public void Ignorer() => Statut = StatutAlerte.Ignoree;

        public bool PeutEtreResolue()
            => Statut != StatutAlerte.Resolue && Statut != StatutAlerte.Ignoree;
    }
}