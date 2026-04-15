using System.Collections.Generic;
using Examen.ApplicationCore.Domain;

namespace Examen.ApplicationCore.Interfaces
{
    public interface IMachineRepository
    {
        IEnumerable<Machine> GetAll();
        Machine? GetById(string codeMachine);     
        void Add(Machine machine);
        void Update(Machine machine);
        void Delete(Machine machine);
    }
}