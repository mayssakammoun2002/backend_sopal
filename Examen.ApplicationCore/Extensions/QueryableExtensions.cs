using System;
using System.Linq;
using System.Threading.Tasks;
using Examen.ApplicationCore.DTOs.Common;
using Microsoft.EntityFrameworkCore;

namespace Examen.ApplicationCore.Extensions
{
    public static class QueryableExtensions
    {
        // ====================== VERSION SYNCHRONE ======================
        // Utilisée par les services synchrones (ex: ServiceResultatControle.GetAllPaginated)
        public static PaginatedResult<T> ToPaginatedResult<T>(
            this IQueryable<T> query, int pageNumber, int pageSize)
        {
            var totalCount = query.Count();
            var items = query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return new PaginatedResult<T>
            {
                Items = items,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount
            };
        }

        // ====================== VERSION ASYNCHRONE ======================
        // À utiliser quand la chaîne d'appel est async (contrôleur/service async)
        public static async Task<PaginatedResult<T>> ToPaginatedResultAsync<T>(
            this IQueryable<T> query, int pageNumber, int pageSize)
        {
            var totalCount = await query.CountAsync();
            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PaginatedResult<T>
            {
                Items = items,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount
            };
        }
    }
}