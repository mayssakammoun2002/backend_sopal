using System.Collections.Generic;
using Examen.ApplicationCore.Domain;

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

        IEnumerable<Produit> GetWithEchantillonnageMinimum(int minTaille);
        IEnumerable<TypeDefaut> GetAllTypeDefauts();
    }
}