using System;
using System.Collections.Generic;
using Examen.ApplicationCore.Domain;
using Examen.ApplicationCore.DTOs;

namespace Examen.ApplicationCore.Interfaces
{
    public interface IServiceResultatControle
    {
        ResultatControle Ajouter(ResultatControleDTO dto);
        ResultatControle Modifier(string id, ResultatControleDTO dto);
        ResultatControle GetById(string id);
        void Delete(string id);

        IEnumerable<ResultatControleResponseDTO> GetAll(int? utilisateurIdConnecte, bool estAdmin);

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