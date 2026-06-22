using Examen.ApplicationCore.Domain;
using Examen.ApplicationCore.Interfaces;
using Microsoft.Extensions.Logging;

namespace Examen.ApplicationCore.Services
{
    /// <summary>
    /// Service principal de gestion des alertes qualité SOPAL.
    ///
    /// Règles de déclenchement :
    ///   1. Règle des 2 non-conformes consécutifs sur la même machine/article → Alerte immédiate (Avertissement).
    ///   2. Seuil de taux : si les 100 derniers contrôles dépassent le seuil configuré (défaut 30 %) → Alerte.
    ///      • 0–9 %  : Avertissement
    ///      • 10–19% : Critique
    ///      • ≥ 20 % : Urgence
    /// </summary>
    public class AlerteService : IServiceAlerte
    {
        // Seuil par défaut (%) si aucun Seuil configuré en base
        private const decimal SEUIL_PAR_DEFAUT = 30m;

        private readonly IUnitOfWork _unitOfWork;
        private readonly INotificationService _notifService;
        private readonly ILogger<AlerteService> _logger;

        public AlerteService(
            IUnitOfWork unitOfWork,
            INotificationService notifService,
            ILogger<AlerteService> logger)
        {
            _unitOfWork = unitOfWork;
            _notifService = notifService;
            _logger = logger;
        }

        // ══════════════════════════════════════════════════════════════
        // CRUD
        // ══════════════════════════════════════════════════════════════

        public void Add(Alerte alerte) => _unitOfWork.Repository<Alerte>().Add(alerte);
        public void Update(Alerte alerte) => _unitOfWork.Repository<Alerte>().Update(alerte);
        public void Delete(Alerte alerte) => _unitOfWork.Repository<Alerte>().Delete(alerte);
        public void Commit() => _unitOfWork.Save();

        public void DeleteById(int id)
        {
            var a = GetById(id);
            if (a != null) _unitOfWork.Repository<Alerte>().Delete(a);
        }

        public IEnumerable<Alerte> GetAll() => _unitOfWork.Repository<Alerte>().GetAll();

        public Alerte? GetById(int id)
            => _unitOfWork.Repository<Alerte>().GetAll().FirstOrDefault(a => a.Id == id);

        // ══════════════════════════════════════════════════════════════
        // ALERTES ACTIVES
        // ══════════════════════════════════════════════════════════════

        public IEnumerable<Alerte> GetActives()
            => _unitOfWork.Repository<Alerte>().GetAll()
                .Where(a => a.Statut == StatutAlerte.Nouvelle || a.Statut == StatutAlerte.EnCours)
                .OrderByDescending(a => a.Niveau)
                .ThenByDescending(a => a.DateAlerte);

        // ══════════════════════════════════════════════════════════════
        // CYCLE DE VIE
        // ══════════════════════════════════════════════════════════════

        public void PrendreEnChargeAlerte(int alerteId, int userId)
        {
            var alerte = GetById(alerteId)
                ?? throw new KeyNotFoundException($"Alerte {alerteId} introuvable.");

            alerte.MarquerEnCours();
            _unitOfWork.Repository<Alerte>().Update(alerte);
            _unitOfWork.Save();

            _logger.LogInformation("🔧 Alerte {Id} prise en charge par userId={UserId}", alerteId, userId);
        }

        public void ResoudreAlerte(int alerteId, int userId, string commentaire)
        {
            var alerte = GetById(alerteId)
                ?? throw new KeyNotFoundException($"Alerte {alerteId} introuvable.");

            alerte.Resoudre(userId, commentaire);
            _unitOfWork.Repository<Alerte>().Update(alerte);
            _unitOfWork.Save();

            _logger.LogInformation("✅ Alerte {Id} résolue par userId={UserId}", alerteId, userId);
        }

        public void IgnorerAlerte(int alerteId, int userId, string raison)
        {
            var alerte = GetById(alerteId)
                ?? throw new KeyNotFoundException($"Alerte {alerteId} introuvable.");

            alerte.Ignorer(userId, raison);
            _unitOfWork.Repository<Alerte>().Update(alerte);
            _unitOfWork.Save();

            _logger.LogInformation("🚫 Alerte {Id} ignorée par userId={UserId}. Raison : {Raison}",
                alerteId, userId, raison);
        }

        // ══════════════════════════════════════════════════════════════
        // COMMENTAIRES
        // ══════════════════════════════════════════════════════════════

        public void AjouterCommentaire(int alerteId, int auteurId, string nomAuteur, string contenu)
        {
            var alerte = GetById(alerteId)
                ?? throw new KeyNotFoundException($"Alerte {alerteId} introuvable.");

            if (string.IsNullOrWhiteSpace(contenu))
                throw new ArgumentException("Le contenu du commentaire ne peut pas être vide.");

            _unitOfWork.Repository<CommentaireAlerte>().Add(new CommentaireAlerte
            {
                AlerteId = alerteId,
                AuteurId = auteurId,
                NomAuteur = nomAuteur,
                Contenu = contenu.Trim(),
                DateCreation = DateTime.UtcNow
            });
            _unitOfWork.Save();

            _logger.LogInformation("💬 Commentaire sur alerte {Id} par {Auteur}", alerteId, nomAuteur);
        }

        public IEnumerable<CommentaireAlerte> GetCommentaires(int alerteId)
            => _unitOfWork.Repository<CommentaireAlerte>().GetAll()
                .Where(c => c.AlerteId == alerteId)
                .OrderByDescending(c => c.DateCreation);

        // ══════════════════════════════════════════════════════════════
        // ① RÈGLE : 2 NON-CONFORMES CONSÉCUTIFS (appelée après chaque saisie)
        // ══════════════════════════════════════════════════════════════

        /// <summary>
        /// À appeler depuis <c>ResultatsControleController</c> après chaque enregistrement
        /// d'un résultat Non Conforme.
        /// Déclenche une alerte si les 2 derniers contrôles de la même machine/article sont NC.
        /// </summary>
        public async Task VerifierNonConformesConsecutifsAsync(
            string codeMachine, string codeArticle, string numOF)
        {
            // Les 2 derniers contrôles pour cette machine + article
            var derniers = _unitOfWork.Repository<ResultatControle>()
                .GetAll()
                .Where(r => r.CodeMachine == codeMachine && r.CodeArticle == codeArticle)
                .OrderByDescending(r => r.DateControle)
                .Take(2)
                .ToList();

            if (derniers.Count < 2)
                return; // Pas assez d'historique

            bool tousNonConformes = derniers.All(r => r.StatutLot == "Non Conforme");
            if (!tousNonConformes)
                return;

            // Éviter doublon : une alerte active sur même machine/article ?
            bool dejaActive = _unitOfWork.Repository<Alerte>().GetAll()
                .Any(a => a.CodeMachine == codeMachine
                       && a.CodeArticle == codeArticle
                       && a.Statut != StatutAlerte.Resolue
                       && a.Statut != StatutAlerte.Ignoree);

            if (dejaActive)
            {
                _logger.LogInformation(
                    "⚠️ Alerte déjà active pour {Machine}/{Article}, doublon ignoré",
                    codeMachine, codeArticle);
                return;
            }

            var alerte = new Alerte
            {
                CodeMachine = codeMachine,
                CodeArticle = codeArticle,
                NumOF = numOF,
                NbNonConformesConsecutifs = 2,
                TauxDetecte = 100,           // 2/2 = 100 % sur les 2 derniers
                QuantiteDefauts = 2,
                QuantiteTotale = 2,
                Niveau = NiveauAlerte.Avertissement,
                Statut = StatutAlerte.Nouvelle,
                Message = $"2 contrôles Non Conformes consécutifs — " +
                          $"Machine : {codeMachine} | Article : {codeArticle} | OF : {numOF}",
                DateAlerte = DateTime.UtcNow
            };

            _unitOfWork.Repository<Alerte>().Add(alerte);
            _unitOfWork.Save();

            _logger.LogWarning(
                "🚨 Alerte #{Id} [2 NC consécutifs] Machine={Machine} Article={Article} OF={OF}",
                alerte.Id, codeMachine, codeArticle, numOF);

            await EnvoyerNotificationSiPossibleAsync(alerte);
        }

        // ══════════════════════════════════════════════════════════════
        // ② RÈGLE : SEUIL DE TAUX (job planifié ou appel API)
        // ══════════════════════════════════════════════════════════════

        /// <summary>
        /// Vérifie tous les seuils actifs configurés en base.
        /// Peut être appelée par un Hosted Service (ex: toutes les 5 min).
        /// </summary>
        public async Task VerifierSeuilsAsync()
        {
            try
            {
                var seuils = _unitOfWork.Repository<Seuil>()
                    .GetAll()
                    .Where(s => s.EstActif)
                    .ToList();

                _logger.LogInformation("🔍 Vérification de {Count} seuils actifs", seuils.Count);

                // Si aucun seuil configuré → appliquer le seuil par défaut 30 %
                if (!seuils.Any())
                {
                    await VerifierSeuilParDefautAsync();
                    return;
                }

                foreach (var seuil in seuils)
                    await VerifierUnSeuilAsync(seuil);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Erreur lors de la vérification des seuils");
            }
        }

        // ── Seuil configuré ──────────────────────────────────────────

        private async Task VerifierUnSeuilAsync(Seuil seuil)
        {
            try
            {
                var query = _unitOfWork.Repository<ResultatControle>()
                    .GetAll()
                    .Where(r => r.CodeMachine == seuil.CodeMachine);

                if (!string.IsNullOrWhiteSpace(seuil.CodeArticle))
                    query = query.Where(r => r.CodeArticle == seuil.CodeArticle);

                var data = query
                    .OrderByDescending(r => r.DateControle)
                    .Take(100)
                    .ToList();

                if (!data.Any()) return;

                var taux = CalculerTaux(data);

                _logger.LogInformation(
                    "📊 Machine {M} | Article {A} → Taux={T:F1}% / Seuil={S}%",
                    seuil.CodeMachine, seuil.CodeArticle ?? "tous", taux, seuil.SeuilPourcentage);

                if (taux < seuil.SeuilPourcentage) return;

                if (AlerteActiveExiste(seuil.CodeMachine, seuil.CodeArticle, seuil.Id)) return;

                var alerte = ConstruireAlerteFromSeuil(seuil, data, taux);
                _unitOfWork.Repository<Alerte>().Add(alerte);
                _unitOfWork.Save();

                await EnvoyerNotificationSiPossibleAsync(alerte);

                _logger.LogWarning(
                    "🚨 Alerte #{Id} [seuil {S}%] Machine={M} → Taux={T:F1}%",
                    alerte.Id, seuil.SeuilPourcentage, alerte.CodeMachine, taux);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Erreur vérification seuil {Id}", seuil.Id);
            }
        }

        // ── Seuil par défaut 30 % (aucun Seuil en base) ─────────────

        private async Task VerifierSeuilParDefautAsync()
        {
            var machines = _unitOfWork.Repository<ResultatControle>()
                .GetAll()
                .Select(r => new { r.CodeMachine, r.CodeArticle })
                .Distinct()
                .ToList();

            foreach (var m in machines)
            {
                var data = _unitOfWork.Repository<ResultatControle>()
                    .GetAll()
                    .Where(r => r.CodeMachine == m.CodeMachine && r.CodeArticle == m.CodeArticle)
                    .OrderByDescending(r => r.DateControle)
                    .Take(100)
                    .ToList();

                if (!data.Any()) continue;

                var taux = CalculerTaux(data);
                if (taux < SEUIL_PAR_DEFAUT) continue;

                if (AlerteActiveExiste(m.CodeMachine, m.CodeArticle)) continue;

                var alerte = new Alerte
                {
                    CodeMachine = m.CodeMachine,
                    CodeArticle = m.CodeArticle,
                    TauxDetecte = taux,
                    QuantiteDefauts = data.Count(r => r.StatutLot == "Non Conforme"),
                    QuantiteTotale = data.Count,
                    Niveau = DeterminerNiveau(taux),
                    Statut = StatutAlerte.Nouvelle,
                    Message = $"Taux NC {taux:F1}% dépasse le seuil {SEUIL_PAR_DEFAUT}% " +
                              $"| Machine : {m.CodeMachine} | Article : {m.CodeArticle}",
                    DateAlerte = DateTime.UtcNow
                };

                _unitOfWork.Repository<Alerte>().Add(alerte);
                _unitOfWork.Save();

                await EnvoyerNotificationSiPossibleAsync(alerte);

                _logger.LogWarning(
                    "🚨 Alerte #{Id} [seuil défaut {S}%] Machine={M} Article={A} → Taux={T:F1}%",
                    alerte.Id, SEUIL_PAR_DEFAUT, m.CodeMachine, m.CodeArticle, taux);
            }
        }

        // ══════════════════════════════════════════════════════════════
        // HELPERS PRIVÉS
        // ══════════════════════════════════════════════════════════════

        private static decimal CalculerTaux(IList<ResultatControle> data)
        {
            if (!data.Any()) return 0;
            int defauts = data.Count(r => r.StatutLot == "Non Conforme");
            return (decimal)defauts / data.Count * 100;
        }

        private static NiveauAlerte DeterminerNiveau(decimal taux)
        {
            if (taux >= 20) return NiveauAlerte.Urgence;
            if (taux >= 10) return NiveauAlerte.Critique;
            return NiveauAlerte.Avertissement;
        }

        private bool AlerteActiveExiste(string codeMachine, string? codeArticle, int? seuilId = null)
        {
            return _unitOfWork.Repository<Alerte>().GetAll()
                .Any(a => a.CodeMachine == codeMachine
                       && a.CodeArticle == codeArticle
                       && (seuilId == null || a.SeuilId == seuilId)
                       && a.Statut != StatutAlerte.Resolue
                       && a.Statut != StatutAlerte.Ignoree);
        }

        private static Alerte ConstruireAlerteFromSeuil(
            Seuil seuil, IList<ResultatControle> data, decimal taux)
        {
            int defauts = data.Count(r => r.StatutLot == "Non Conforme");
            return new Alerte
            {
                SeuilId = seuil.Id,
                CodeMachine = seuil.CodeMachine,
                CodeArticle = seuil.CodeArticle,
                TauxDetecte = taux,
                QuantiteDefauts = defauts,
                QuantiteTotale = data.Count,
                Niveau = DeterminerNiveau(taux),
                Statut = StatutAlerte.Nouvelle,
                Message = $"Taux NC {taux:F1}% dépasse le seuil {seuil.SeuilPourcentage}% " +
                          $"| Machine : {seuil.CodeMachine}" +
                          (seuil.CodeArticle != null ? $" | Article : {seuil.CodeArticle}" : ""),
                DateAlerte = DateTime.UtcNow
            };
        }

        private async Task EnvoyerNotificationSiPossibleAsync(Alerte alerte)
        {
            try
            {
                await _notifService.EnvoyerAlerteAsync(alerte);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "❌ Échec notification pour alerte #{Id} — alerte créée quand même", alerte.Id);
            }
        }
    }
}