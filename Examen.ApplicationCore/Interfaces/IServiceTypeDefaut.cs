using Examen.ApplicationCore.Domain;
using System.Collections.Generic;

namespace Examen.ApplicationCore.Interfaces
{
    public interface IServiceTypeDefaut
    {
        IEnumerable<TypeDefaut> GetAll();
        TypeDefaut GetById(int id);
        void Add(TypeDefaut typeDefaut);
        void Update(TypeDefaut typeDefaut);
        void Delete(TypeDefaut typeDefaut);
        void DeleteById(int id);
        void Commit();
    }
}