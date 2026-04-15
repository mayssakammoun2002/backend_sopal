using System.Collections.Generic;
using Examen.ApplicationCore.Domain;

namespace Examen.ApplicationCore.Repository
{

    public interface IResultatControleRepository
    {
        void Add(ResultatControle resultat);
        IEnumerable<ResultatControle> GetAll();
        ResultatControle GetById(string id);
        void Delete(ResultatControle resultat);

        IEnumerable<ResultatControle> GetByMachine(string codeMachine);
        IEnumerable<ResultatControle> GetByProduit(string codeArticle);
    }
}