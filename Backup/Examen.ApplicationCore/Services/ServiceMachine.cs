using System;
using System.Collections.Generic;
using AM.ApplicationCore.Interfaces;
using Examen.ApplicationCore.Domain;
using Examen.ApplicationCore.Interfaces;

namespace Examen.ApplicationCore.Services
{
    public class ServiceMachine : IServiceMachine
    {
        private readonly IUnitOfWork _unitOfWork;

        public ServiceMachine(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        public IEnumerable<Machine> GetAll()
        {
            return _unitOfWork.Repository<Machine>().GetAll();
        }

        public Machine? GetById(string codeMachine)
        {
            if (string.IsNullOrWhiteSpace(codeMachine))
                return null;

            return _unitOfWork.Repository<Machine>().GetById(codeMachine);
        }

        public void Add(Machine machine)
        {
            ArgumentNullException.ThrowIfNull(machine);
            _unitOfWork.Repository<Machine>().Add(machine);
        }

        public void Update(Machine machine)
        {
            ArgumentNullException.ThrowIfNull(machine);
            _unitOfWork.Repository<Machine>().Update(machine);
        }

        public void Delete(Machine machine)
        {
            ArgumentNullException.ThrowIfNull(machine);
            _unitOfWork.Repository<Machine>().Delete(machine);
        }

        public void DeleteById(string codeMachine)
        {
            var machine = GetById(codeMachine);
            if (machine != null)
            {
                Delete(machine);
            }
        }

        public void Commit()
        {
            _unitOfWork.Save();
        }
    }
}