using Examen.ApplicationCore.Domain;
using Examen.ApplicationCore.Interfaces;
using Microsoft.Extensions.Logging;

namespace Examen.ApplicationCore.Services
{
    public class AlerteService : IServiceAlerte
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly INotificationService _notifService;
        private readonly ILogger<AlerteService> _logger;

        public AlerteService(IUnitOfWork unitOfWork,
                             INotificationService notifService,
                             ILogger<AlerteService> logger)
        {
            _unitOfWork = unitOfWork;
            _notifService = notifService;
            _logger = logger;
        }

        // ══════════════════════════════════════════
        // MÉTHODES CRUD
        // ══════════════════════════════════════════

        public void Add(Alerte alerte)
        {
            _unitOfWork.Repository<Alerte>().Add(alerte);
        }

        public void Update(Alerte alerte)
        {
            _unitOfWork.Repository<Alerte>().Update(alerte);
        }

        public void Delete(Alerte alerte)
        {
            _unitOfWork.Repository<Alerte>().Delete(alerte);
        }

        public void DeleteById(int id)
        {
            var alerte = GetById(id);
            if (alerte != null)
                _unitOfWork.Repository<Alerte>().Delete(alerte);
        }

        public void Commit()
        {
            _unitOfWork.Save();
        }

        public IEnumerable<Alerte> GetAll()
        {
            return _unitOfWork.Repository<Alerte>().GetAll();
        }

        public Alerte? GetById(int id)
        {
            return _unitOfWork.Repository<Alerte>().GetAll()
                .FirstOrDefault(a => a.Id == id);
        }

        // ══════════════════════════════════════════
        // LISTE DES ALERTES ACTIVES
        // ══════════════════════════════════════════

        /// <summary>
        /// Retourne toutes les alertes dont le statut est Nouvelle ou EnCours.
        /// </summary>
        public IEnumerable<Alerte> GetActives()
        {
            return _unitOfWork.Repository<Alerte>().GetAll()
                .Where(a => a.Statut == StatutAlerte.Nouvelle
                         || a.Statut == StatutAlerte.EnCours)
                .OrderByDescending(a => a.Niveau)
                .ThenByDescending(a => a.DateAlerte);
        }

        // ══════════════════════════════════════════
        // CYCLE DE VIE D'UNE ALERTE
        // ══════════════════════════════════════════

        /// <summary>
        /// Passe l'alerte en statut EnCours et l'assigne à l'utilisateur.
        /// </summary>
        public void PrendreEnChargeAlerte(int alerteId, int userId)
        {
            var alerte = GetById(alerteId)
                ?? throw new KeyNotFoundException($"Alerte {alerteId} introuvable.");

            if (alerte.Statut == StatutAlerte.Resolue || alerte.Statut == StatutAlerte.Ignoree)
                throw new InvalidOperationException(
                    $"Impossible de prendre en charge une alerte déjà {alerte.Statut}.");

            alerte.MarquerEnCours();

            _unitOfWork.Repository<Alerte>().Update(alerte);
            _unitOfWork.Save();

            _logger.LogInformation("🔧 Alerte {Id} prise en charge par utilisateur {UserId}",
                alerteId, userId);
        }

        /// <summary>
        /// Résout une alerte et enregistre le commentaire de résolution.
        /// </summary>
        public void ResoudreAlerte(int alerteId, int userId, string commentaire)
        {
            var alerte = GetById(alerteId)
                ?? throw new KeyNotFoundException($"Alerte {alerteId} introuvable.");

            if (!alerte.PeutEtreResolue())
                throw new InvalidOperationException(
                    $"L'alerte {alerteId} est déjà {alerte.Statut} et ne peut pas être résolue.");

            alerte.Resoudre(userId, commentaire);

            _unitOfWork.Repository<Alerte>().Update(alerte);
            _unitOfWork.Save();

            _logger.LogInformation("✅ Alerte {Id} résolue par utilisateur {UserId}", alerteId, userId);
        }

        /// <summary>
        /// Ignore une alerte et enregistre la raison.
        /// </summary>
        public void IgnorerAlerte(int alerteId, int userId, string raison)
        {
            var alerte = GetById(alerteId)
                ?? throw new KeyNotFoundException($"Alerte {alerteId} introuvable.");

            if (alerte.Statut == StatutAlerte.Resolue || alerte.Statut == StatutAlerte.Ignoree)
                throw new InvalidOperationException(
                    $"L'alerte {alerteId} est déjà {alerte.Statut}.");

            alerte.Ignorer();
            alerte.CommentaireResolution = raison;
            alerte.ResolueParId = userId;
            alerte.DateResolution = DateTime.UtcNow;

            _unitOfWork.Repository<Alerte>().Update(alerte);
            _unitOfWork.Save();

            _logger.LogInformation("🚫 Alerte {Id} ignorée par utilisateur {UserId}. Raison : {Raison}",
                alerteId, userId, raison);
        }

        // ══════════════════════════════════════════
        // COMMENTAIRES
        // ══════════════════════════════════════════

        /// <summary>
        /// Ajoute un commentaire à une alerte existante.
        /// </summary>
        public void AjouterCommentaire(int alerteId, int auteurId, string nomAuteur, string contenu)
        {
            var alerte = GetById(alerteId)
                ?? throw new KeyNotFoundException($"Alerte {alerteId} introuvable.");

            if (string.IsNullOrWhiteSpace(contenu))
                throw new ArgumentException("Le contenu du commentaire ne peut pas être vide.");

            var commentaire = new CommentaireAlerte
            {
                AlerteId = alerteId,
                AuteurId = auteurId,
                NomAuteur = nomAuteur,
                Contenu = contenu.Trim(),
                DateCreation = DateTime.UtcNow
            };

            _unitOfWork.Repository<CommentaireAlerte>().Add(commentaire);
            _unitOfWork.Save();

            _logger.LogInformation("💬 Commentaire ajouté à l'alerte {Id} par {Auteur}", alerteId, nomAuteur);
        }

        /// <summary>
        /// Retourne tous les commentaires d'une alerte, du plus récent au plus ancien.
        /// </summary>
        public IEnumerable<CommentaireAlerte> GetCommentaires(int alerteId)
        {
            return _unitOfWork.Repository<CommentaireAlerte>().GetAll()
                .Where(c => c.AlerteId == alerteId)
                .OrderByDescending(c => c.DateCreation);
        }

        // ══════════════════════════════════════════
        // VÉRIFICATION AUTOMATIQUE DES SEUILS
        // ══════════════════════════════════════════

        public async Task VerifierSeuilsAsync()
        {
            try
            {
                var seuils = _unitOfWork.Repository<Seuil>()
                    .GetAll()
                    .Where(s => s.EstActif)
                    .ToList();

                _logger.LogInformation("🔍 Vérification de {Count} seuils actifs", seuils.Count);

                foreach (var seuil in seuils)
                {
                    await VerifierUnSeuilAsync(seuil);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Erreur lors de la vérification des seuils");
            }
        }

        private async Task VerifierUnSeuilAsync(Seuil seuil)
        {
            try
            {
                // Filtre sur CodeMachine ET CodeArticle si renseigné
                var query = _unitOfWork.Repository<ResultatControle>()
                    .GetAll()
                    .Where(r => r.CodeMachine == seuil.CodeMachine);

                if (!string.IsNullOrWhiteSpace(seuil.CodeArticle))
                    query = query.Where(r => r.CodeArticle == seuil.CodeArticle);

                var data = query
                    .OrderByDescending(r => r.DateControle)
                    .Take(100)
                    .ToList();

                if (!data.Any())
                {
                    _logger.LogDebug("⚠️ Aucun résultat de contrôle pour machine {Machine}", seuil.CodeMachine);
                    return;
                }

                var total = data.Count;
                var defauts = data.Count(r => r.StatutLot == "Defectueux");
                var taux = (decimal)defauts / total * 100;

                _logger.LogInformation(
                    "📊 Machine {Machine} | Article {Article} : Taux={Taux:F2}%, Seuil={Seuil}%",
                    seuil.CodeMachine, seuil.CodeArticle ?? "tous", taux, seuil.SeuilPourcentage);

                if (taux < seuil.SeuilPourcentage)
                    return;

                // Éviter les doublons : une alerte active sur ce seuil suffit
                var alerteExistante = _unitOfWork.Repository<Alerte>()
                    .GetAll()
                    .FirstOrDefault(a => a.SeuilId == seuil.Id
                                      && a.Statut != StatutAlerte.Resolue
                                      && a.Statut != StatutAlerte.Ignoree);

                if (alerteExistante != null)
                {
                    _logger.LogInformation("⚠️ Alerte déjà active pour seuil {Id}, ignorée", seuil.Id);
                    return;
                }

                var alerte = new Alerte
                {
                    SeuilId = seuil.Id,
                    CodeMachine = seuil.CodeMachine,
                    CodeArticle = seuil.CodeArticle,
                    TauxDetecte = taux,
                    QuantiteDefauts = defauts,
                    QuantiteTotale = total,
                    Niveau = DeterminerNiveauAlerte(taux),
                    Statut = StatutAlerte.Nouvelle,
                    Message = $"Taux {taux:F2}% dépasse le seuil {seuil.SeuilPourcentage}% " +
                              $"sur machine {seuil.CodeMachine}" +
                              (seuil.CodeArticle != null ? $" / article {seuil.CodeArticle}" : ""),
                    DateAlerte = DateTime.UtcNow
                };

                _unitOfWork.Repository<Alerte>().Add(alerte);
                _unitOfWork.Save();

                // Envoi de la notification (ne doit pas bloquer la création de l'alerte)
                try
                {
                    await _notifService.EnvoyerAlerteAsync(alerte);
                }
                catch (Exception notifEx)
                {
                    _logger.LogError(notifEx,
                        "❌ Échec envoi notification pour alerte {Id} — alerte créée quand même", alerte.Id);
                }

                _logger.LogWarning(
                    "🚨 Alerte #{Id} créée : Machine {Machine} → Taux {Taux:F2}% (Seuil: {Seuil}%)",
                    alerte.Id, alerte.CodeMachine, taux, seuil.SeuilPourcentage);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Erreur lors de la vérification du seuil {Id}", seuil.Id);
            }
        }

        // ══════════════════════════════════════════
        // HELPERS PRIVÉS
        // ══════════════════════════════════════════

        /// <summary>
        /// Détermine le niveau d'alerte en fonction du taux de défauts.
        /// </summary>
        private static NiveauAlerte DeterminerNiveauAlerte(decimal taux)
        {
            if (taux >= 20) return NiveauAlerte.Urgence;
            if (taux >= 10) return NiveauAlerte.Critique;
            return NiveauAlerte.Avertissement;
        }
    }
}