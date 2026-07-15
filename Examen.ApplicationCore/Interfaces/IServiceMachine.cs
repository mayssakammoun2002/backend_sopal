using System.Collections.Generic;
using System.Threading.Tasks;
using Examen.ApplicationCore.Domain;
using Examen.ApplicationCore.DTOs.Common;

namespace Examen.ApplicationCore.Interfaces
{
    public interface IServiceMachine
    {
        IEnumerable<Machine> GetAll();
        Machine? GetById(string codeMachine);
        void Add(Machine machine);
        void Update(Machine machine);
        void Delete(Machine machine);
        void DeleteById(string codeMachine);
        void Commit();

        Task<PaginatedResult<Machine>> GetMachinesPagineesAsync(PaginationParams paginationParams);
    }
}