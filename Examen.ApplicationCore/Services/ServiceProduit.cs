using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AM.ApplicationCore.Interfaces;
using Examen.ApplicationCore.Domain;
using Examen.ApplicationCore.DTOs.Common;
using Examen.ApplicationCore.Extensions;
using Examen.ApplicationCore.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Examen.ApplicationCore.Services
{
    public class ServiceProduit : IServiceProduit
    {
        private readonly IUnitOfWork _unitOfWork;
        public ServiceProduit(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        public IEnumerable<Produit> GetAll()
        {
            return _unitOfWork.Repository<Produit>().GetAll();
        }

        public Produit? GetById(string codeArticle)
        {
            if (string.IsNullOrWhiteSpace(codeArticle))
                return null;
            return _unitOfWork.Repository<Produit>().GetById(codeArticle);
        }

        public void Add(Produit produit)
        {
            ArgumentNullException.ThrowIfNull(produit);
            _unitOfWork.Repository<Produit>().Add(produit);
        }

        public void Update(Produit produit)
        {
            ArgumentNullException.ThrowIfNull(produit);
            _unitOfWork.Repository<Produit>().Update(produit);
        }

        public void Delete(Produit produit)
        {
            ArgumentNullException.ThrowIfNull(produit);
            _unitOfWork.Repository<Produit>().Delete(produit);
        }

        public void DeleteById(string codeArticle)
        {
            var produit = GetById(codeArticle);
            if (produit != null)
            {
                Delete(produit);
            }
        }

        public void Commit()
        {
            _unitOfWork.Save();
        }

        public IEnumerable<TypeDefaut> GetAllTypeDefauts()
        {
            return _unitOfWork.Repository<TypeDefaut>().GetAll();
        }

        public IEnumerable<Produit> GetWithEchantillonnageMinimum(int minTaille)
        {
            if (minTaille < 0)
                minTaille = 0;

            if (_unitOfWork.Repository<Produit>() is IProduitRepository customRepo)
            {
                return customRepo.GetWithEchantillonnageMinimum(minTaille);
            }

            return _unitOfWork.Repository<Produit>()
                .GetAll()
                .Where(p => p.TailleEchantillonnage >= minTaille);
        }

        // ───────────────────────────────────────────────
        // Pagination + recherche
        // ───────────────────────────────────────────────
        public async Task<PaginatedResult<Produit>> GetProduitsPaginesAsync(PaginationParams paginationParams)
        {
            var query = _unitOfWork.Repository<Produit>()
                .Query()
                .AsNoTracking();

            if (!string.IsNullOrWhiteSpace(paginationParams.Recherche))
            {
                var recherche = paginationParams.Recherche.ToLower();
                query = query.Where(p =>
                    p.CodeArticle.ToLower().Contains(recherche) ||
                    p.NomProduit.ToLower().Contains(recherche) ||
                    (p.Designation != null && p.Designation.ToLower().Contains(recherche)));
            }

            query = query.OrderBy(p => p.CodeArticle);

            return await query.ToPaginatedResultAsync(
                paginationParams.PageNumber,
                paginationParams.PageSize);
        }
    }
}