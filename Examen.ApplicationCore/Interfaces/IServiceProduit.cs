using System.Collections.Generic;
using System.Threading.Tasks;
using Examen.ApplicationCore.Domain;
using Examen.ApplicationCore.DTOs.Common;

namespace Examen.ApplicationCore.Interfaces
{
    public interface IServiceProduit
    {
        IEnumerable<Produit> GetAll();
        Produit? GetById(string codeArticle);
        void Add(Produit produit);
        void Update(Produit produit);
        void Delete(Produit produit);
        void DeleteById(string codeArticle);
        void Commit();

        IEnumerable<TypeDefaut> GetAllTypeDefauts();
        IEnumerable<Produit> GetWithEchantillonnageMinimum(int minTaille);

        Task<PaginatedResult<Produit>> GetProduitsPaginesAsync(PaginationParams paginationParams);
    }
}