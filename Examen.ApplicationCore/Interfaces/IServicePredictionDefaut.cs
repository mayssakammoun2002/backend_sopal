using System.Collections.Generic;
using System.Threading.Tasks;
using Examen.ApplicationCore.Domain;

namespace Examen.ApplicationCore.Interfaces
{
    public interface IServicePredictionDefaut
    {
        IEnumerable<PredictionDefaut> GetAll();

        PredictionDefaut? GetById(int id);

        IEnumerable<PredictionDefaut> GetByResultatControle(string resultatControleId);

        void Add(PredictionDefaut predictionDefaut);

        void Update(PredictionDefaut predictionDefaut);

        void Delete(PredictionDefaut predictionDefaut);

        void DeleteById(int id);

        void Commit();

        Task<string> PredireAsync(object data);
    }
}