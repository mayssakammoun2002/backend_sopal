using Examen.ApplicationCore.Domain;

namespace Examen.ApplicationCore.Domain
{
    public class DestinataireNotification
    {
        public int Id { get; set; }

        /// <summary>Cible un utilisateur spécifique (optionnel).</summary>
        public int? UtilisateurId { get; set; }

        /// <summary>Cible un rôle entier, ex : "Responsable", "Admin" (optionnel).</summary>
        public string? Role { get; set; }

        public CanalNotification Canal { get; set; }

        public NiveauAlerte NiveauMinimum { get; set; } = NiveauAlerte.Avertissement;

        public bool EstActif { get; set; } = true;

        /// <summary>Adresse email ou numéro de téléphone selon le canal.</summary>
        public string Destinataire { get; set; } = string.Empty;

        // ── Navigation ────────────────────────────────────────────────────
        public Utilisateur? Utilisateur { get; set; }
    }
}