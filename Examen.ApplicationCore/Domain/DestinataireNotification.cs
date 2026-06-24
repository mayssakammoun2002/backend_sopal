using Examen.ApplicationCore.Domain;

namespace Examen.ApplicationCore.Domain
{
    public class DestinataireNotification
    {
        public int Id { get; set; }

        /// <summary>Cible un utilisateur spécifique (optionnel).</summary>
        public int? UtilisateurId { get; set; }

        public string? Role { get; set; }

        public CanalNotification Canal { get; set; }

        public NiveauAlerte NiveauMinimum { get; set; } = NiveauAlerte.Avertissement;

        public bool EstActif { get; set; } = true;

        public string Destinataire { get; set; } = string.Empty;

        public Utilisateur? Utilisateur { get; set; }
    }
}