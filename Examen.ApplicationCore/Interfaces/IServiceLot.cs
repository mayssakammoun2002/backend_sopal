using Examen.ApplicationCore.Domain;

namespace Examen.ApplicationCore.Interfaces
{
    public interface IServiceLot
    {
        IEnumerable<Lot> GetAll();
        IEnumerable<Lot> GetByMachine(string machineId);
        IEnumerable<Lot> GetByStatut(string statut);
        Lot? GetById(int id);
        void Add(Lot lot);
        void Update(Lot lot);
        void Delete(Lot lot);
        void DeleteById(int id);
        void Commit();
    }
}