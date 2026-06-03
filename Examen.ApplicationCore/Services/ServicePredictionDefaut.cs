using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using AM.ApplicationCore.Interfaces;
using Examen.ApplicationCore.Domain;
using Examen.ApplicationCore.Interfaces;
using Newtonsoft.Json;

namespace Examen.ApplicationCore.Services
{
    public class ServicePredictionDefaut : IServicePredictionDefaut
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly HttpClient _httpClient;

        public ServicePredictionDefaut(
            IUnitOfWork unitOfWork,
            HttpClient httpClient)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        }

        public IEnumerable<PredictionDefaut> GetAll()
        {
            return _unitOfWork.Repository<PredictionDefaut>().GetAll();
        }

        public PredictionDefaut? GetById(int id)
        {
            return _unitOfWork.Repository<PredictionDefaut>().GetById(id);
        }

        public IEnumerable<PredictionDefaut> GetByResultatControle(string resultatControleId)
        {
            if (string.IsNullOrWhiteSpace(resultatControleId))
                return Enumerable.Empty<PredictionDefaut>();

            return _unitOfWork.Repository<PredictionDefaut>()
                .GetAll()
                .Where(p => p.ResultatControleId == resultatControleId);
        }

        public void Add(PredictionDefaut predictionDefaut)
        {
            ArgumentNullException.ThrowIfNull(predictionDefaut);

            _unitOfWork.Repository<PredictionDefaut>().Add(predictionDefaut);
        }

        public void Update(PredictionDefaut predictionDefaut)
        {
            ArgumentNullException.ThrowIfNull(predictionDefaut);

            _unitOfWork.Repository<PredictionDefaut>().Update(predictionDefaut);
        }

        public void Delete(PredictionDefaut predictionDefaut)
        {
            ArgumentNullException.ThrowIfNull(predictionDefaut);

            _unitOfWork.Repository<PredictionDefaut>().Delete(predictionDefaut);
        }

        public void DeleteById(int id)
        {
            var prediction = GetById(id);

            if (prediction != null)
            {
                Delete(prediction);
            }
        }

        public void Commit()
        {
            _unitOfWork.Save();
        }

        // ───────────────────────────────────────────────
        // Méthodes spécifiques
        // ───────────────────────────────────────────────

        public async Task<string> PredireAsync(object data)
        {
            ArgumentNullException.ThrowIfNull(data);

            var json = JsonConvert.SerializeObject(data);

            var content = new StringContent(
                json,
                Encoding.UTF8,
                "application/json"
            );

            var response = await _httpClient.PostAsync(
                "http://127.0.0.1:8000/predict",
                content
            );

            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsStringAsync();
        }
    }
}