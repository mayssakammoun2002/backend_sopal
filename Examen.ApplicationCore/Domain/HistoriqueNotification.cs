namespace Examen.ApplicationCore.Domain
{
    public class HistoriqueNotification
    {
        public int Id { get; set; }
        public int AlerteId { get; set; }
        public Alerte? Alerte { get; set; }
        public CanalNotification Canal { get; set; }
        public string Destinataire { get; set; } = string.Empty;
        public string Sujet { get; set; } = string.Empty;
        public string Corps { get; set; } = string.Empty;
        public StatutNotification Statut { get; set; } = StatutNotification.EnAttente;
        public int NbTentatives { get; set; }
        public DateTime? DateEnvoi { get; set; }
        public DateTime? DateLecture { get; set; }
        public string? ErreurMessage { get; set; }
        public int UtilisateurId { get; set; }
    }
}