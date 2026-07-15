using System.Collections.Generic;
using System.Threading.Tasks;
using Examen.ApplicationCore.Domain;
using Examen.ApplicationCore.DTOs.Common;

namespace Examen.ApplicationCore.Interfaces
{
    public interface IServiceTypeDefaut
    {
        IEnumerable<TypeDefaut> GetAll();
        TypeDefaut? GetById(int id);
        void Add(TypeDefaut typeDefaut);
        void Update(TypeDefaut typeDefaut);
        void Delete(TypeDefaut typeDefaut);
        void DeleteById(int id);
        void Commit();

        Task<PaginatedResult<TypeDefaut>> GetTypeDefautsPaginesAsync(PaginationParams paginationParams);
    }
}