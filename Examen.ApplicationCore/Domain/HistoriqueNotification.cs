using System;

namespace Examen.ApplicationCore.Domain
{
    public class HistoriqueNotification
    {
        public int Id { get; set; }
        public int AlerteId { get; set; }
        public int UtilisateurId { get; set; }
        public CanalNotification Canal { get; set; }
        public string Destinataire { get; set; } = string.Empty;
        public string? Sujet { get; set; }
        public string Corps { get; set; } = string.Empty;
        public StatutNotification Statut { get; set; } = StatutNotification.EnAttente;
        public DateTime? DateEnvoi { get; set; }
        public DateTime? DateLecture { get; set; }
        public string? ErreurMessage { get; set; }
        public int NbTentatives { get; set; } = 0;

        // Navigation
        public Alerte Alerte { get; set; } = null!;
        public Utilisateur? Utilisateur { get; set; }
    }

    public enum CanalNotification { Email = 0, SMS = 1, InApp = 2 }
    public enum StatutNotification { EnAttente = 0, Envoye = 1, Echec = 2, Lu = 3 }
}