using System.Collections.Generic;
using System.Threading.Tasks;
using Examen.ApplicationCore.Domain;

namespace Examen.ApplicationCore.Interfaces
{
    public interface IProduitRepository // : IRepository<Produit>   ← optionnel
    {
        // Méthodes génériques (souvent déjà dans IRepository<Produit>)
        IEnumerable<Produit> GetAll();
        Produit? GetById(string codeArticle);
        void Add(Produit produit);
        void Update(Produit produit);
        void Delete(Produit produit);

        // Méthodes spécifiques
        IEnumerable<Produit> GetWithEchantillonnageMinimum(int minTaille);

    }
}