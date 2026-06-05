using Examen.ApplicationCore.Domain;
using Examen.ApplicationCore.Interfaces;
using AM.ApplicationCore.Interfaces;

namespace Examen.ApplicationCore.Services
{
    public class ServiceLot : IServiceLot
    {
        private readonly IUnitOfWork _unitOfWork;

        public ServiceLot(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        public IEnumerable<Lot> GetAll()
        {
            return _unitOfWork.Repository<Lot>().GetAll();
        }

        public IEnumerable<Lot> GetByMachine(string machineId)
        {
            if (string.IsNullOrWhiteSpace(machineId))
                return Enumerable.Empty<Lot>();

            return _unitOfWork.Repository<Lot>()
                              .GetAll()
                              .Where(l => l.MachineId == machineId);
        }

        public IEnumerable<Lot> GetByStatut(string statut)
        {
            if (string.IsNullOrWhiteSpace(statut))
                return Enumerable.Empty<Lot>();

            return _unitOfWork.Repository<Lot>()
                              .GetAll()
                              .Where(l => l.Statut == statut);
        }

        public Lot? GetById(int id)
        {
            if (id <= 0) return null;
            return _unitOfWork.Repository<Lot>().GetById(id);
        }

        public void Add(Lot lot)
        {
            ArgumentNullException.ThrowIfNull(lot);
            _unitOfWork.Repository<Lot>().Add(lot);
        }

        public void Update(Lot lot)
        {
            ArgumentNullException.ThrowIfNull(lot);
            _unitOfWork.Repository<Lot>().Update(lot);
        }

        public void Delete(Lot lot)
        {
            ArgumentNullException.ThrowIfNull(lot);
            _unitOfWork.Repository<Lot>().Delete(lot);
        }

        public void DeleteById(int id)
        {
            var lot = GetById(id);
            if (lot != null)
                Delete(lot);
        }

        public void Commit()
        {
            _unitOfWork.Save();
        }
    }
}