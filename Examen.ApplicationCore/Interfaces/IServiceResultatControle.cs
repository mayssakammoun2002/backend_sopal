using System;
using Examen.ApplicationCore.Domain;
using Examen.ApplicationCore.DTOs;
using Examen.ApplicationCore.DTOs.Common;

namespace Examen.ApplicationCore.Interfaces
{
    public interface IServiceResultatControle
    {
        ResultatControle Ajouter(ResultatControleDTO dto);
        ResultatControle Modifier(string id, ResultatControleDTO dto);
        ResultatControle GetById(string id);
        void Delete(string id);

        PaginatedResult<ResultatControleResponseDTO> GetAllPaginated(
            int? utilisateurIdConnecte,
            bool estAdmin,
            PaginationParams paginationParams,
            string? codeMachine = null,
            string? statut = null,
            DateTime? dateDebut = null,
            DateTime? dateFin = null,
            string? recherche = null);

        void Commit();

        ResultatControleStatsDTO GetStats(
            string? codeMachine,
            string? statut,
            DateTime? dateDebut,
            DateTime? dateFin,
            int? utilisateurIdConnecte,
            bool estAdmin);
    }
}