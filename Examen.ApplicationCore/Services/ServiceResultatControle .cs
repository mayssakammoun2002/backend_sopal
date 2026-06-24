using System;
using System.Collections.Generic;
using System.Linq;
using Examen.ApplicationCore.Domain;
using Examen.ApplicationCore.DTOs;
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

        // ====================== GET ALL ======================
        public IEnumerable<ResultatControleResponseDTO> GetAll(int? utilisateurIdConnecte, bool estAdmin)
        {
            try
            {
                var query = _unitOfWork.Repository<ResultatControle>().GetAll().AsQueryable();

                // Un opérateur ne voit que ses propres contrôles, l'admin voit tout
                if (!estAdmin && utilisateurIdConnecte.HasValue)
                    query = query.Where(r => r.UtilisateurId == utilisateurIdConnecte.Value);

                var resultats = query.ToList();

                var machines = _unitOfWork.Repository<Machine>().GetAll()
                    .ToDictionary(m => m.CodeMachine.Trim().ToUpper(), m => m.NomMachine ?? "N/A");

                var utilisateurs = _unitOfWork.Repository<Utilisateur>().GetAll()
                    .ToDictionary(u => u.Id, u =>
                        string.IsNullOrWhiteSpace(u.FirstName) && string.IsNullOrWhiteSpace(u.LastName)
                            ? "Inconnu"
                            : $"{u.FirstName} {u.LastName}".Trim());

                return resultats.Select(r => new ResultatControleResponseDTO
                {
                    Id = r.Id ?? "",
                    DateControle = r.DateControle,
                    CodeMachine = r.CodeMachine ?? "",
                    NomMachine = machines.TryGetValue((r.CodeMachine ?? "").Trim().ToUpper(), out var nomMachine)
                                ? nomMachine : "N/A",
                    CodeArticle = r.CodeArticle ?? "",
                    NomProduit = "N/A",
                    UtilisateurId = r.UtilisateurId,
                    Controleur = utilisateurs.TryGetValue(r.UtilisateurId, out var nomControleur)
                                ? nomControleur : "Inconnu",
                    NumOF = r.NumOF ?? "",
                    NumLotMatiere = r.NumLotMatiere,
                    Quantite = r.Quantite,
                    Cadence = r.Cadence,
                    NbEchantillons = r.NbEchantillons,
                    StatutLot = r.StatutLot ?? "Non Conforme",
                    NbDefautsTest1 = r.NbDefautsTest1,
                    NbDefautsTest2 = r.NbDefautsTest2,
                    SolutionGlobale = r.SolutionGlobale,
                    Defaut1 = r.Defaut1,
                    Defaut2 = r.Defaut2
                }).ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERREUR GetAll: {ex.Message}");
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
            if (dto.Quantite <= 0)
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
                Quantite = dto.Quantite,
                Cadence = dto.Cadence,

                UtilisateurId = dto.UtilisateurId.Value,
                NbEchantillons = dto.NbEchantillons > 0 ? dto.NbEchantillons : 3,
                NbDefautsTest1 = dto.NbDefautsTest1,
                NbDefautsTest2 = dto.NbDefautsTest2,
                SolutionGlobale = string.IsNullOrWhiteSpace(dto.SolutionGlobale)
                    ? null : dto.SolutionGlobale.Trim(),
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
            if (dto.Quantite <= 0)
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
            existing.Quantite = dto.Quantite;
            existing.Cadence = dto.Cadence;
            existing.UtilisateurId = dto.UtilisateurId.Value;
            existing.NbEchantillons = dto.NbEchantillons > 0 ? dto.NbEchantillons : existing.NbEchantillons;
            existing.NbDefautsTest1 = dto.NbDefautsTest1;
            existing.NbDefautsTest2 = dto.NbDefautsTest2;
            existing.SolutionGlobale = string.IsNullOrWhiteSpace(dto.SolutionGlobale)
                ? null : dto.SolutionGlobale.Trim();
            existing.Defaut1 = string.IsNullOrWhiteSpace(dto.Defaut1) ? null : dto.Defaut1.Trim();
            existing.Defaut2 = string.IsNullOrWhiteSpace(dto.Defaut2) ? null : dto.Defaut2.Trim();
            existing.StatutLot = (dto.NbDefautsTest1 + dto.NbDefautsTest2) <= 1 ? "Conforme" : "Non Conforme";

            _unitOfWork.Repository<ResultatControle>().Update(existing);
            _unitOfWork.Save();

            return existing;
        }

        // ====================== GET BY ID ======================
        public ResultatControle GetById(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentException("L'ID ne peut pas être vide.");

            return _unitOfWork.Repository<ResultatControle>().GetById(id)
                   ?? throw new KeyNotFoundException($"ResultatControle '{id}' introuvable.");
        }

        // ====================== DELETE ======================
        public void Delete(string id)
        {
            var resultat = GetById(id);
            _unitOfWork.Repository<ResultatControle>().Delete(resultat);
            _unitOfWork.Save();
        }

        // ====================== STATS (DASHBOARD) ======================
        public ResultatControleStatsDTO GetStats(
            string? codeMachine,
            string? statut,
            DateTime? dateDebut,
            DateTime? dateFin,
            int? utilisateurIdConnecte,
            bool estAdmin)
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

                // Un opérateur ne voit que ses propres contrôles, l'admin voit tout
                if (!estAdmin && utilisateurIdConnecte.HasValue)
                    query = query.Where(r => r.UtilisateurId == utilisateurIdConnecte.Value);

                var resultats = query.ToList();

                var machines = _unitOfWork.Repository<Machine>().GetAll()
                    .ToDictionary(m => m.CodeMachine.Trim().ToUpper(), m => m.NomMachine ?? "N/A");

                Dictionary<int, string> typesDefauts;
                try
                {
                    typesDefauts = _unitOfWork.Repository<TypeDefaut>().GetAll()
                        .ToDictionary(t => t.Id, t => string.IsNullOrWhiteSpace(t.NomDefaut) ? $"Défaut {t.Id}" : t.NomDefaut);
                }
                catch
                {
                    typesDefauts = new Dictionary<int, string>();
                }

                int total = resultats.Count;
                int conformes = resultats.Count(r => r.StatutLot == "Conforme");
                int nonConformes = total - conformes;
                int totalDefauts = resultats.Sum(r => r.NbDefautsTest1 + r.NbDefautsTest2);
                long quantiteTotaleRealisee = resultats.Sum(r => (long)r.Quantite);

                // Taux de "soudure" = non conformité / quantité totale réalisée (en %)
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
                        TauxConformite = g.Count() == 0 ? 0 : Math.Round(g.Count(x => x.StatutLot == "Conforme") * 100.0 / g.Count(), 1)
                    })
                    .OrderByDescending(x => x.TotalControles)
                    .ToList();

                var defautCounts = new Dictionary<string, int>();
                foreach (var r in resultats)
                {
                    if (r.NbDefautsTest1 > 0)
                    {
                        string libelle = r.TypeDefaut1Id.HasValue && typesDefauts.TryGetValue(r.TypeDefaut1Id.Value, out var l1)
                            ? l1
                            : (!string.IsNullOrWhiteSpace(r.Defaut1) ? r.Defaut1! : "Défaut non précisé");

                        defautCounts[libelle] = defautCounts.GetValueOrDefault(libelle, 0) + r.NbDefautsTest1;
                    }

                    if (r.NbDefautsTest2 > 0)
                    {
                        string libelle = r.TypeDefaut2Id.HasValue && typesDefauts.TryGetValue(r.TypeDefaut2Id.Value, out var l2)
                            ? l2
                            : (!string.IsNullOrWhiteSpace(r.Defaut2) ? r.Defaut2! : "Défaut non précisé");

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

        // ====================== ID UNIQUE ======================
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