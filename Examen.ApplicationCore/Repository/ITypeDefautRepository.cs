using System.Collections.Generic;
using Examen.ApplicationCore.Domain;

namespace Examen.ApplicationCore.Interfaces
{
    public interface ITypeDefautRepository
    {
        IEnumerable<TypeDefaut> GetAll();
        TypeDefaut GetById(int id);

        void Add(TypeDefaut typeDefaut);
        void Update(TypeDefaut typeDefaut);
        void Delete(TypeDefaut typeDefaut);
    }
}