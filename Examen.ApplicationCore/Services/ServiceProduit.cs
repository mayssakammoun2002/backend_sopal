using System;
using System.Collections.Generic;
using System.Linq;
using AM.ApplicationCore.Interfaces;
using Examen.ApplicationCore.Domain;
using Examen.ApplicationCore.Interfaces;

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

        // ───────────────────────────────────────────────
        // Méthodes spécifiques
        // ───────────────────────────────────────────────

        public IEnumerable<TypeDefaut> GetAllTypeDefauts()
        {
            return _unitOfWork.Repository<TypeDefaut>().GetAll();
        }

        public IEnumerable<Produit> GetWithEchantillonnageMinimum(int minTaille)
        {
            if (minTaille < 0)
                minTaille = 0;

            // Si repository personnalisé existe
            if (_unitOfWork.Repository<Produit>() is IProduitRepository customRepo)
            {
                return customRepo.GetWithEchantillonnageMinimum(minTaille);
            }

            // Fallback
            return _unitOfWork.Repository<Produit>()
                .GetAll()
                .Where(p => p.TailleEchantillonnage >= minTaille);
        }
    }
}