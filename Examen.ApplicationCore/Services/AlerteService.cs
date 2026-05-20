using Examen.ApplicationCore.Domain;
using Examen.ApplicationCore.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Examen.ApplicationCore.Services
{
    public class AlerteService : IServiceAlerte
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly INotificationService _notifService;
        private readonly ILogger<AlerteService> _logger;

        // ══════════════════════════════════════════
        // TRANSITIONS DE STATUT AUTORISÉES
        // ══════════════════════════════════════════

        private static readonly Dictionary<StatutAlerte, StatutAlerte[]> _transitionsAutorisees = new()
        {
            { StatutAlerte.Nouvelle,  new[] { StatutAlerte.EnCours, StatutAlerte.Ignoree } },
            { StatutAlerte.EnCours,   new[] { StatutAlerte.Resolue, StatutAlerte.Ignoree } },
            { StatutAlerte.Resolue,   Array.Empty<StatutAlerte>() },
            { StatutAlerte.Ignoree,   Array.Empty<StatutAlerte>() },
        };

        public AlerteService(
            IUnitOfWork unitOfWork,
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
            => _unitOfWork.Repository<Alerte>().Add(alerte);

        public void Update(Alerte alerte)
            => _unitOfWork.Repository<Alerte>().Update(alerte);

        public void Delete(Alerte alerte)
            => _unitOfWork.Repository<Alerte>().Delete(alerte);

        public void DeleteById(int id)
        {
            var alerte = GetById(id);
            if (alerte != null)
                _unitOfWork.Repository<Alerte>().Delete(alerte);
        }

        public void Commit()
            => _unitOfWork.Save();

        public IEnumerable<Alerte> GetAll()
            => _unitOfWork.Repository<Alerte>().GetAll().ToList();

        public Alerte? GetById(int id)
            => _unitOfWork.Repository<Alerte>().GetById(id);

        public IEnumerable<Alerte> GetActives()
            => _unitOfWork.Repository<Alerte>()
                .Find(a => a.Statut == StatutAlerte.Nouvelle || a.Statut == StatutAlerte.EnCours)
                .OrderByDescending(a => a.DateAlerte)
                .ToList();

        // ══════════════════════════════════════════
        // TRANSITIONS DE STATUT
        // ══════════════════════════════════════════

        /// <summary>Passe une alerte "Nouvelle" en "EnCours"</summary>
        public void PrendreEnChargeAlerte(int alerteId, int userId)
        {
            var alerte = GetAlerteOuException(alerteId);
            ValiderTransition(alerte.Statut, StatutAlerte.EnCours);

            alerte.Statut = StatutAlerte.EnCours;
            _unitOfWork.Repository<Alerte>().Update(alerte);
            _unitOfWork.Save();

            AjouterCommentaireInterne(alerte, userId, $"Prise en charge par l'utilisateur {userId}");

            _logger.LogInformation("🔄 Alerte {AlerteId} prise en charge par utilisateur {UserId}", alerteId, userId);
        }

        /// <summary>Résout une alerte (EnCours → Resolue)</summary>
        public void ResoudreAlerte(int alerteId, int userId, string commentaire)
        {
            var alerte = GetAlerteOuException(alerteId);
            ValiderTransition(alerte.Statut, StatutAlerte.Resolue);

            alerte.Statut = StatutAlerte.Resolue;
            alerte.DateResolution = DateTime.UtcNow;
            alerte.ResolueParId = userId;
            alerte.CommentaireResolution = commentaire;

            _unitOfWork.Repository<Alerte>().Update(alerte);
            _unitOfWork.Save();

            AjouterCommentaireInterne(alerte, userId, $"Résolu : {commentaire}");

            _logger.LogInformation("✅ Alerte {AlerteId} résolue par utilisateur {UserId}", alerteId, userId);
        }

        /// <summary>Ignore une alerte (Nouvelle ou EnCours → Ignoree)</summary>
        public void IgnorerAlerte(int alerteId, int userId, string raison)
        {
            var alerte = GetAlerteOuException(alerteId);
            ValiderTransition(alerte.Statut, StatutAlerte.Ignoree);

            alerte.Statut = StatutAlerte.Ignoree;
            _unitOfWork.Repository<Alerte>().Update(alerte);
            _unitOfWork.Save();

            AjouterCommentaireInterne(alerte, userId, $"Ignorée : {raison}");

            _logger.LogInformation("🚫 Alerte {AlerteId} ignorée par utilisateur {UserId}", alerteId, userId);
        }

        // ══════════════════════════════════════════
        // GESTION DES COMMENTAIRES
        // ══════════════════════════════════════════

        public void AjouterCommentaire(int alerteId, int auteurId, string nomAuteur, string contenu)
        {
            // Vérifier que l'alerte existe
            _ = GetAlerteOuException(alerteId);

            if (string.IsNullOrWhiteSpace(contenu))
                throw new ArgumentException("Le commentaire ne peut pas être vide.");

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

            _logger.LogInformation("💬 Commentaire ajouté sur alerte {AlerteId} par {NomAuteur}", alerteId, nomAuteur);
        }

        public IEnumerable<CommentaireAlerte> GetCommentaires(int alerteId)
            => _unitOfWork.Repository<CommentaireAlerte>()
                .Find(c => c.AlerteId == alerteId)
                .OrderBy(c => c.DateCreation)
                .ToList();

        // ══════════════════════════════════════════
        // VÉRIFICATION AUTOMATIQUE DES SEUILS
        // ══════════════════════════════════════════

        public async Task VerifierSeuilsAsync()
        {
            try
            {
                var seuils = _unitOfWork.Repository<Seuil>()
                    .Find(s => s.EstActif)
                    .ToList();

                _logger.LogInformation("🔍 Vérification de {Count} seuils actifs", seuils.Count);

                foreach (var seuil in seuils)
                {
                    await VerifierUnSeuilAsync(seuil);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Erreur globale lors de la vérification des seuils");
            }
        }

        private async Task VerifierUnSeuilAsync(Seuil seuil)
        {
            try
            {
                // ✅ Filtrage côté base de données (pas de GetAll())
                var data = _unitOfWork.Repository<ResultatControle>()
                    .Find(r => r.CodeMachine == seuil.CodeMachine)
                    .OrderByDescending(r => r.DateControle)
                    .Take(100)
                    .ToList();

                if (!data.Any())
                {
                    _logger.LogDebug("⚠️ Aucun résultat de contrôle pour machine {CodeMachine}", seuil.CodeMachine);
                    return;
                }

                var total = data.Count;
                var defauts = data.Count(r => r.StatutLot == "Defectueux");
                var taux = (decimal)defauts / total * 100;

                _logger.LogInformation("📊 Machine {CodeMachine}: Taux={Taux:F2}%, Seuil={Seuil}%",
                    seuil.CodeMachine, taux, seuil.SeuilPourcentage);

                if (taux < seuil.SeuilPourcentage)
                    return;

                // Anti-doublon : alerte déjà active pour ce seuil ?
                var alerteExistante = _unitOfWork.Repository<Alerte>()
                    .Find(a => a.SeuilId == seuil.Id
                            && a.Statut != StatutAlerte.Resolue
                            && a.Statut != StatutAlerte.Ignoree)
                    .FirstOrDefault();

                if (alerteExistante != null)
                {
                    _logger.LogInformation("⚠️ Alerte déjà active (id={Id}) pour seuil {SeuilId}, ignorée",
                        alerteExistante.Id, seuil.Id);
                    return;
                }

                // Créer la nouvelle alerte
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
                    Message = $"Taux {taux:F2}% dépasse le seuil de {seuil.SeuilPourcentage}%",
                    DateAlerte = DateTime.UtcNow
                };

                _unitOfWork.Repository<Alerte>().Add(alerte);
                _unitOfWork.Save();

                await _notifService.EnvoyerAlerteAsync(alerte);

                _logger.LogWarning("🚨 Alerte créée : Machine {CodeMachine} → Taux {Taux:F2}% (Seuil: {Seuil}%)",
                    alerte.CodeMachine, taux, seuil.SeuilPourcentage);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Erreur lors de la vérification du seuil {SeuilId}", seuil.Id);
            }
        }

        // ══════════════════════════════════════════
        // HELPERS PRIVÉS
        // ══════════════════════════════════════════

        private Alerte GetAlerteOuException(int alerteId)
        {
            var alerte = GetById(alerteId);
            if (alerte == null)
                throw new KeyNotFoundException($"Alerte {alerteId} introuvable.");
            return alerte;
        }

        private void ValiderTransition(StatutAlerte actuel, StatutAlerte cible)
        {
            if (!_transitionsAutorisees[actuel].Contains(cible))
                throw new InvalidOperationException(
                    $"Transition '{actuel}' → '{cible}' non autorisée.");
        }

        /// <summary>Ajoute un commentaire système sans sauvegarder (Save déjà fait par l'appelant)</summary>
        private void AjouterCommentaireInterne(Alerte alerte, int userId, string message)
        {
            var commentaire = new CommentaireAlerte
            {
                AlerteId = alerte.Id,
                AuteurId = userId,
                NomAuteur = $"Système (user {userId})",
                Contenu = message,
                DateCreation = DateTime.UtcNow
            };
            _unitOfWork.Repository<CommentaireAlerte>().Add(commentaire);
            _unitOfWork.Save();
        }

        private static NiveauAlerte DeterminerNiveauAlerte(decimal taux) => taux switch
        {
            >= 20 => NiveauAlerte.Urgence,
            >= 10 => NiveauAlerte.Critique,
            _ => NiveauAlerte.Avertissement
        };
    }
}