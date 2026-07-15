using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Examen.ApplicationCore.Domain;
using Examen.ApplicationCore.DTOs.Common;
using Examen.ApplicationCore.Extensions;
using Examen.ApplicationCore.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Examen.ApplicationCore.Services
{
    public class ServiceTypeDefaut : IServiceTypeDefaut
    {
        private readonly IUnitOfWork _unitOfWork;
        public ServiceTypeDefaut(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        public IEnumerable<TypeDefaut> GetAll()
        {
            return _unitOfWork.Repository<TypeDefaut>().GetAll();
        }

        public TypeDefaut? GetById(int id)
        {
            if (id <= 0) return null;
            return _unitOfWork.Repository<TypeDefaut>().GetById(id);
        }

        public void Add(TypeDefaut typeDefaut)
        {
            ArgumentNullException.ThrowIfNull(typeDefaut);
            _unitOfWork.Repository<TypeDefaut>().Add(typeDefaut);
        }

        public void Update(TypeDefaut typeDefaut)
        {
            ArgumentNullException.ThrowIfNull(typeDefaut);
            _unitOfWork.Repository<TypeDefaut>().Update(typeDefaut);
        }

        public void Delete(TypeDefaut typeDefaut)
        {
            ArgumentNullException.ThrowIfNull(typeDefaut);
            _unitOfWork.Repository<TypeDefaut>().Delete(typeDefaut);
        }

        public void DeleteById(int id)
        {
            var entity = GetById(id);
            if (entity != null)
            {
                Delete(entity);
            }
        }

        public void Commit()
        {
            _unitOfWork.Save();
        }

        // ───────────────────────────────────────────────
        // Pagination + recherche
        // ───────────────────────────────────────────────
        public async Task<PaginatedResult<TypeDefaut>> GetTypeDefautsPaginesAsync(PaginationParams paginationParams)
        {
            var query = _unitOfWork.Repository<TypeDefaut>()
                .Query()
                .AsNoTracking();

            if (!string.IsNullOrWhiteSpace(paginationParams.Recherche))
            {
                var recherche = paginationParams.Recherche.ToLower();
                query = query.Where(d =>
                    d.NomDefaut.ToLower().Contains(recherche) ||
                    (d.Description != null && d.Description.ToLower().Contains(recherche)));
            }

            query = query.OrderBy(d => d.NomDefaut);

            return await query.ToPaginatedResultAsync(
                paginationParams.PageNumber,
                paginationParams.PageSize);
        }
    }
}