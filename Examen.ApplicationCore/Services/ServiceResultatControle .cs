using System;
using System.Collections.Generic;
using System.Linq;
using Examen.ApplicationCore.Domain;
using Examen.ApplicationCore.DTOs;
using Examen.ApplicationCore.DTOs.Common;
using Examen.ApplicationCore.Extensions;
using Examen.ApplicationCore.Interfaces;

namespace Examen.ApplicationCore.Services
{
    public class ServiceResultatControle : IServiceResultatControle
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly Random _random = new Random();

        public ServiceResultatControle(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        // ====================== HELPERS CONTROLEURS ======================

        private static List<int> ParseControleurIds(string? ids)
        {
            if (string.IsNullOrWhiteSpace(ids))
                return new List<int>();

            return ids.Split(',', StringSplitOptions.RemoveEmptyEntries)
                      .Select(s => int.TryParse(s.Trim(), out var v) ? v : 0)
                      .Where(v => v > 0)
                      .ToList();
        }

        /*
         * Construit la chaîne des noms de contrôleurs.
         * Si le front n'envoie que les IDs, on résout les noms en base.
         */
        private string? ResoudreNomsControleurs(
            List<int>? controleurIds,
            string? controleurNoms,
            Dictionary<int, string>? cacheUtilisateurs = null)
        {
            if (!string.IsNullOrWhiteSpace(controleurNoms))
                return controleurNoms.Trim();

            if (controleurIds == null || controleurIds.Count == 0)
                return null;

            var utilisateurs = cacheUtilisateurs ?? _unitOfWork.Repository<Utilisateur>().GetAll()
                .ToDictionary(u => u.Id, u =>
                    string.IsNullOrWhiteSpace(u.FirstName) && string.IsNullOrWhiteSpace(u.LastName)
                        ? "Inconnu"
                        : $"{u.FirstName} {u.LastName}".Trim());

            var noms = controleurIds
                .Select(id => utilisateurs.TryGetValue(id, out var n) ? n : null)
                .Where(n => !string.IsNullOrWhiteSpace(n))
                .ToList();

            return noms.Count == 0 ? null : string.Join(", ", noms);
        }

        // ====================== GET ALL (PAGINÉ + FILTRÉ) ======================

        public PaginatedResult<ResultatControleResponseDTO> GetAllPaginated(
            int? utilisateurIdConnecte, bool estAdmin, PaginationParams paginationParams,
            string? codeMachine, string? statut, DateTime? dateDebut, DateTime? dateFin, string? recherche)
        {
            try
            {
                IQueryable<ResultatControle> query =
                    _unitOfWork.Repository<ResultatControle>().GetAll().AsQueryable();

                /*
                 * ⚠️ Plus aucun filtre par utilisateur connecté :
                 * tout utilisateur authentifié voit l'historique complet.
                 */

                if (!string.IsNullOrWhiteSpace(codeMachine))
                    query = query.Where(r => r.CodeMachine.Trim().ToUpper() == codeMachine.Trim().ToUpper());
                if (!string.IsNullOrWhiteSpace(statut))
                    query = query.Where(r => r.StatutLot == statut);
                if (dateDebut.HasValue)
                    query = query.Where(r => r.DateControle >= dateDebut.Value);
                if (dateFin.HasValue)
                    query = query.Where(r => r.DateControle <= dateFin.Value);

                query = query.OrderByDescending(r => r.DateControle);

                var machines = _unitOfWork.Repository<Machine>().GetAll()
                    .ToDictionary(m => m.CodeMachine.Trim().ToUpper(), m => m.NomMachine ?? "N/A");

                var utilisateurs = _unitOfWork.Repository<Utilisateur>().GetAll()
                    .ToDictionary(u => u.Id, u =>
                        string.IsNullOrWhiteSpace(u.FirstName) && string.IsNullOrWhiteSpace(u.LastName)
                            ? "Inconnu"
                            : $"{u.FirstName} {u.LastName}".Trim());

                var resultatsFiltres = query.ToList();

                // ---------- RECHERCHE GLOBALE ----------
                if (!string.IsNullOrWhiteSpace(recherche))
                {
                    var terme = recherche.Trim().ToLower();

                    resultatsFiltres = resultatsFiltres.Where(r =>
                        (r.NumOF ?? "").ToLower().Contains(terme) ||
                        (r.CodeArticle ?? "").ToLower().Contains(terme) ||
                        (r.CodeMachine ?? "").ToLower().Contains(terme) ||
                        (machines.TryGetValue((r.CodeMachine ?? "").Trim().ToUpper(), out var nm)
                            && nm.ToLower().Contains(terme)) ||
                        (r.NumLotMatiere ?? "").ToLower().Contains(terme) ||
                        (r.NumConteneur ?? "").ToLower().Contains(terme) ||
                        (r.StatutLot ?? "").ToLower().Contains(terme) ||
                        (r.Defaut1 ?? "").ToLower().Contains(terme) ||
                        (r.Defaut2 ?? "").ToLower().Contains(terme) ||
                        (r.ControleurNoms ?? "").ToLower().Contains(terme) ||
                        (utilisateurs.TryGetValue(r.UtilisateurId, out var nomSaisie)
                            && nomSaisie.ToLower().Contains(terme))
                    ).ToList();
                }

                int totalCount = resultatsFiltres.Count;
                int pageNumber = paginationParams.PageNumber < 1 ? 1 : paginationParams.PageNumber;
                int pageSize = paginationParams.PageSize < 1 ? 10 : paginationParams.PageSize;

                var pageItems = resultatsFiltres
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                var items = pageItems.Select(r =>
                {
                    var ids = ParseControleurIds(r.ControleurIds);

                    // Priorité aux contrôleurs sélectionnés ;
                    // repli sur les IDs, puis sur l'utilisateur de saisie.
                    var nomsControleurs = !string.IsNullOrWhiteSpace(r.ControleurNoms)
                        ? r.ControleurNoms!
                        : (ResoudreNomsControleurs(ids, null, utilisateurs)
                           ?? (utilisateurs.TryGetValue(r.UtilisateurId, out var f) ? f : "—"));

                    return new ResultatControleResponseDTO
                    {
                        Id = r.Id ?? "",
                        DateControle = r.DateControle,
                        CodeMachine = r.CodeMachine ?? "",
                        NomMachine = machines.TryGetValue((r.CodeMachine ?? "").Trim().ToUpper(), out var nomMachine)
                                    ? nomMachine : "N/A",
                        CodeArticle = r.CodeArticle ?? "",
                        NomProduit = "N/A",
                        UtilisateurId = r.UtilisateurId,
                        SaisiPar = utilisateurs.TryGetValue(r.UtilisateurId, out var nomSaisiPar)
                                    ? nomSaisiPar : "Inconnu",
                        Controleur = nomsControleurs,
                        ControleurIds = ids,
                        NumOF = r.NumOF ?? "",
                        NumLotMatiere = r.NumLotMatiere,
                        NumConteneur = r.NumConteneur,
                        Quantite = r.Quantite,
                        Cadence = r.Cadence,
                        NbEchantillons = r.NbEchantillons,
                        StatutLot = r.StatutLot ?? "Non Conforme",
                        NbDefautsTest1 = r.NbDefautsTest1,
                        NbDefautsTest2 = r.NbDefautsTest2,
                        SolutionGlobale = r.SolutionGlobale,
                        Defaut1 = r.Defaut1,
                        Defaut2 = r.Defaut2
                    };
                }).ToList();

                return new PaginatedResult<ResultatControleResponseDTO>
                {
                    Items = items,
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    TotalCount = totalCount
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERREUR GetAllPaginated: {ex.Message}");
                throw;
            }
        }

        // ====================== AJOUTER ======================

        public ResultatControle Ajouter(ResultatControleDTO dto)
        {
            if (dto == null) throw new ArgumentNullException(nameof(dto));

            if (string.IsNullOrWhiteSpace(dto.CodeMachine))
                throw new ArgumentException("CodeMachine est obligatoire.");
            if (string.IsNullOrWhiteSpace(dto.CodeArticle))
                throw new ArgumentException("CodeArticle est obligatoire.");
            if (string.IsNullOrWhiteSpace(dto.NumOF))
                throw new ArgumentException("NumOF est obligatoire.");
            if (dto.UtilisateurId == null || dto.UtilisateurId <= 0)
                throw new ArgumentException("UtilisateurId est obligatoire.");
            if (dto.ControleurIds == null || dto.ControleurIds.Count == 0)
                throw new ArgumentException("Au moins un contrôleur doit être sélectionné.");
            if (dto.Quantite.HasValue && dto.Quantite.Value <= 0)
                throw new ArgumentException("Quantite doit être supérieure à 0.");
            if (dto.Cadence <= 0)
                throw new ArgumentException("Cadence doit être supérieure à 0.");

            var machine = _unitOfWork.Repository<Machine>().GetAll()
                .FirstOrDefault(m => m.CodeMachine.Trim().ToUpper() == dto.CodeMachine.Trim().ToUpper());
            if (machine == null)
                throw new ArgumentException($"Machine '{dto.CodeMachine}' introuvable.");

            var utilisateur = _unitOfWork.Repository<Utilisateur>().GetAll()
                .FirstOrDefault(u => u.Id == dto.UtilisateurId.Value);
            if (utilisateur == null)
                throw new ArgumentException($"Utilisateur ID {dto.UtilisateurId} introuvable.");

            var resultat = new ResultatControle
            {
                Id = GenerateUniqueId(),
                DateControle = dto.DateControle ?? DateTime.UtcNow,
                CodeMachine = machine.CodeMachine,
                CodeArticle = dto.CodeArticle.ToUpper().Trim(),
                NumOF = dto.NumOF.Trim(),
                NumLotMatiere = string.IsNullOrWhiteSpace(dto.NumLotMatiere) ? null : dto.NumLotMatiere.Trim(),
                NumConteneur = string.IsNullOrWhiteSpace(dto.NumConteneur) ? null : dto.NumConteneur.Trim(),
                Quantite = dto.Quantite ?? 0,
                Cadence = dto.Cadence,
                UtilisateurId = dto.UtilisateurId.Value,

                ControleurIds = string.Join(",", dto.ControleurIds),
                ControleurNoms = ResoudreNomsControleurs(dto.ControleurIds, dto.ControleurNoms),

                NbEchantillons = dto.NbEchantillons > 0 ? dto.NbEchantillons : 3,
                NbDefautsTest1 = dto.NbDefautsTest1,
                NbDefautsTest2 = dto.NbDefautsTest2,
                SolutionGlobale = string.IsNullOrWhiteSpace(dto.SolutionGlobale) ? null : dto.SolutionGlobale.Trim(),
                Defaut1 = string.IsNullOrWhiteSpace(dto.Defaut1) ? null : dto.Defaut1.Trim(),
                Defaut2 = string.IsNullOrWhiteSpace(dto.Defaut2) ? null : dto.Defaut2.Trim(),
                StatutLot = (dto.NbDefautsTest1 + dto.NbDefautsTest2) <= 1 ? "Conforme" : "Non Conforme"
            };

            _unitOfWork.Repository<ResultatControle>().Add(resultat);
            _unitOfWork.Save();
            return resultat;
        }

        // ====================== MODIFIER ======================

        public ResultatControle Modifier(string id, ResultatControleDTO dto)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentException("L'ID est obligatoire.");
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            var existing = GetById(id);

            if (string.IsNullOrWhiteSpace(dto.CodeMachine))
                throw new ArgumentException("CodeMachine est obligatoire.");
            if (string.IsNullOrWhiteSpace(dto.CodeArticle))
                throw new ArgumentException("CodeArticle est obligatoire.");
            if (string.IsNullOrWhiteSpace(dto.NumOF))
                throw new ArgumentException("NumOF est obligatoire.");
            if (dto.UtilisateurId == null || dto.UtilisateurId <= 0)
                throw new ArgumentException("UtilisateurId est obligatoire.");
            if (dto.ControleurIds == null || dto.ControleurIds.Count == 0)
                throw new ArgumentException("Au moins un contrôleur doit être sélectionné.");
            if (dto.Quantite.HasValue && dto.Quantite.Value <= 0)
                throw new ArgumentException("Quantite doit être supérieure à 0.");
            if (dto.Cadence <= 0)
                throw new ArgumentException("Cadence doit être supérieure à 0.");

            var machine = _unitOfWork.Repository<Machine>().GetAll()
                .FirstOrDefault(m => m.CodeMachine.Trim().ToUpper() == dto.CodeMachine.Trim().ToUpper());
            if (machine == null)
                throw new ArgumentException($"Machine '{dto.CodeMachine}' introuvable.");

            var utilisateur = _unitOfWork.Repository<Utilisateur>().GetAll()
                .FirstOrDefault(u => u.Id == dto.UtilisateurId.Value);
            if (utilisateur == null)
                throw new ArgumentException($"Utilisateur ID {dto.UtilisateurId} introuvable.");

            existing.CodeMachine = machine.CodeMachine;
            existing.CodeArticle = dto.CodeArticle.ToUpper().Trim();
            existing.NumOF = dto.NumOF.Trim();
            existing.NumLotMatiere = string.IsNullOrWhiteSpace(dto.NumLotMatiere) ? null : dto.NumLotMatiere.Trim();
            existing.NumConteneur = string.IsNullOrWhiteSpace(dto.NumConteneur) ? null : dto.NumConteneur.Trim();
            existing.Quantite = dto.Quantite ?? existing.Quantite;
            existing.Cadence = dto.Cadence;
            existing.UtilisateurId = dto.UtilisateurId.Value;

            existing.ControleurIds = string.Join(",", dto.ControleurIds);
            existing.ControleurNoms = ResoudreNomsControleurs(dto.ControleurIds, dto.ControleurNoms);

            existing.NbEchantillons = dto.NbEchantillons > 0 ? dto.NbEchantillons : existing.NbEchantillons;
            existing.NbDefautsTest1 = dto.NbDefautsTest1;
            existing.NbDefautsTest2 = dto.NbDefautsTest2;
            existing.SolutionGlobale = string.IsNullOrWhiteSpace(dto.SolutionGlobale) ? null : dto.SolutionGlobale.Trim();
            existing.Defaut1 = string.IsNullOrWhiteSpace(dto.Defaut1) ? null : dto.Defaut1.Trim();
            existing.Defaut2 = string.IsNullOrWhiteSpace(dto.Defaut2) ? null : dto.Defaut2.Trim();
            existing.StatutLot = (dto.NbDefautsTest1 + dto.NbDefautsTest2) <= 1 ? "Conforme" : "Non Conforme";

            _unitOfWork.Repository<ResultatControle>().Update(existing);
            _unitOfWork.Save();
            return existing;
        }

        public ResultatControle GetById(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentException("L'ID ne peut pas être vide.");
            return _unitOfWork.Repository<ResultatControle>().GetById(id)
                   ?? throw new KeyNotFoundException($"ResultatControle '{id}' introuvable.");
        }

        public void Delete(string id)
        {
            var resultat = GetById(id);
            _unitOfWork.Repository<ResultatControle>().Delete(resultat);
            _unitOfWork.Save();
        }

        // ====================== STATS ======================

        public ResultatControleStatsDTO GetStats(
            string? codeMachine, string? statut, DateTime? dateDebut, DateTime? dateFin,
            int? utilisateurIdConnecte, bool estAdmin)
        {
            try
            {
                var query = _unitOfWork.Repository<ResultatControle>().GetAll().AsQueryable();

                if (!string.IsNullOrWhiteSpace(codeMachine))
                    query = query.Where(r => r.CodeMachine.Trim().ToUpper() == codeMachine.Trim().ToUpper());
                if (!string.IsNullOrWhiteSpace(statut))
                    query = query.Where(r => r.StatutLot == statut);
                if (dateDebut.HasValue)
                    query = query.Where(r => r.DateControle >= dateDebut.Value);
                if (dateFin.HasValue)
                    query = query.Where(r => r.DateControle <= dateFin.Value);

                // Statistiques globales : aucun cloisonnement par utilisateur.

                var resultats = query.ToList();

                var machines = _unitOfWork.Repository<Machine>().GetAll()
                    .ToDictionary(m => m.CodeMachine.Trim().ToUpper(), m => m.NomMachine ?? "N/A");

                Dictionary<int, string> typesDefauts;
                try
                {
                    typesDefauts = _unitOfWork.Repository<TypeDefaut>().GetAll()
                        .ToDictionary(t => t.Id, t => string.IsNullOrWhiteSpace(t.NomDefaut) ? $"Défaut {t.Id}" : t.NomDefaut);
                }
                catch { typesDefauts = new Dictionary<int, string>(); }

                int total = resultats.Count;
                int conformes = resultats.Count(r => r.StatutLot == "Conforme");
                int nonConformes = total - conformes;
                int totalDefauts = resultats.Sum(r => r.NbDefautsTest1 + r.NbDefautsTest2);
                long quantiteTotaleRealisee = resultats.Sum(r => (long)r.Quantite);
                double tauxSoudure = quantiteTotaleRealisee == 0
                    ? 0
                    : Math.Round(nonConformes * 100.0 / quantiteTotaleRealisee, 2);

                var parMachine = resultats
                    .GroupBy(r => r.CodeMachine ?? "INCONNU")
                    .Select(g => new StatParMachineDTO
                    {
                        CodeMachine = g.Key,
                        NomMachine = machines.TryGetValue(g.Key.Trim().ToUpper(), out var nom) ? nom : "N/A",
                        TotalControles = g.Count(),
                        Conformes = g.Count(x => x.StatutLot == "Conforme"),
                        NonConformes = g.Count(x => x.StatutLot != "Conforme"),
                        TauxConformite = g.Count() == 0
                            ? 0
                            : Math.Round(g.Count(x => x.StatutLot == "Conforme") * 100.0 / g.Count(), 1)
                    })
                    .OrderByDescending(x => x.TotalControles)
                    .ToList();

                var defautCounts = new Dictionary<string, int>();
                foreach (var r in resultats)
                {
                    if (r.NbDefautsTest1 > 0)
                    {
                        string libelle = !string.IsNullOrWhiteSpace(r.Defaut1)
                            ? r.Defaut1!
                            : "Défaut non précisé";
                        defautCounts[libelle] = defautCounts.GetValueOrDefault(libelle, 0) + r.NbDefautsTest1;
                    }
                    if (r.NbDefautsTest2 > 0)
                    {
                        string libelle = !string.IsNullOrWhiteSpace(r.Defaut2)
                            ? r.Defaut2!
                            : "Défaut non précisé";
                        defautCounts[libelle] = defautCounts.GetValueOrDefault(libelle, 0) + r.NbDefautsTest2;
                    }
                }

                var parTypeDefaut = defautCounts
                    .Select(kv => new StatParDefautDTO { Libelle = kv.Key, Occurrences = kv.Value })
                    .OrderByDescending(x => x.Occurrences)
                    .ToList();

                var evolution = resultats
                    .GroupBy(r => r.DateControle.Date)
                    .Select(g => new StatParJourDTO
                    {
                        Date = g.Key,
                        TotalControles = g.Count(),
                        NonConformes = g.Count(x => x.StatutLot != "Conforme")
                    })
                    .OrderBy(x => x.Date)
                    .ToList();

                return new ResultatControleStatsDTO
                {
                    TotalControles = total,
                    TotalConformes = conformes,
                    TotalNonConformes = nonConformes,
                    TauxConformite = total == 0 ? 0 : Math.Round(conformes * 100.0 / total, 1),
                    TotalDefauts = totalDefauts,
                    QuantiteTotaleRealisee = quantiteTotaleRealisee,
                    TauxSoudure = tauxSoudure,
                    ParMachine = parMachine,
                    ParTypeDefaut = parTypeDefaut,
                    Evolution = evolution
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERREUR GetStats: {ex.Message}");
                throw;
            }
        }

        private string GenerateUniqueId()
        {
            const int maxAttempts = 50;
            var repo = _unitOfWork.Repository<ResultatControle>();
            for (int i = 0; i < maxAttempts; i++)
            {
                string id = "RC" + _random.Next(100000, 999999);
                if (!repo.GetAll().Any(r => r.Id == id))
                    return id;
            }
            throw new InvalidOperationException("Impossible de générer un ID unique.");
        }

        public void Commit() => _unitOfWork.Save();
    }
}