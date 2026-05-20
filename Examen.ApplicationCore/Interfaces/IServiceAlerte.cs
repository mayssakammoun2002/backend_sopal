using System.Collections.Generic;
using System.Threading.Tasks;
using Examen.ApplicationCore.Domain;

namespace Examen.ApplicationCore.Interfaces
{
    public interface IServiceAlerte
    {
        // CRUD
        IEnumerable<Alerte> GetAll();
        Alerte? GetById(int id);
        void Add(Alerte alerte);
        void Update(Alerte alerte);
        void Delete(Alerte alerte);
        void DeleteById(int id);
        void Commit();

        // Métier
        void ResoudreAlerte(int alerteId, int userId, string commentaire);
        Task VerifierSeuilsAsync();
    }
}