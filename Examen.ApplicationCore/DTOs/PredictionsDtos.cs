namespace Examen.ApplicationCore.DTOs
{
    // ── Requête envoyée à FastAPI ─────────────────────────────────────────────
    public class FastApiPredictionRequest
    {
        public string CodeMachine { get; set; } = string.Empty;
        public string CodeArticle { get; set; } = string.Empty;
        public float Quantite { get; set; }
        public float Cadence { get; set; }
        public int NbEchantillons { get; set; }
        public int NbDefautsTest1 { get; set; }
        public int NbDefautsTest2 { get; set; }
    }

    // ── Réponse reçue de FastAPI ──────────────────────────────────────────────
    public class FastApiPredictionResponse
    {
        public bool EstDefectueux { get; set; }
        public float Probabilite { get; set; }
        public int TypeDefautId { get; set; }
        public string NiveauRisque { get; set; } = string.Empty;
        public string Horodatage { get; set; } = string.Empty;
        public string ModelVersion { get; set; } = string.Empty;
        public Dictionary<string, float>? ShapExplication { get; set; }
    }

    // ── Requête batch ────────────────────────────────────────────────────────
    public class FastApiBatchRequest
    {
        public List<FastApiPredictionRequest> Lots { get; set; } = new();
    }

    // ── Réponse batch ────────────────────────────────────────────────────────
    public class FastApiBatchResponse
    {
        public List<FastApiPredictionResponse> Resultats { get; set; } = new();
        public int NbTotal { get; set; }
        public int NbDefectueux { get; set; }
        public float TauxDefaut { get; set; }
        public string Horodatage { get; set; } = string.Empty;
    }

    // ── Statistiques dashboard ───────────────────────────────────────────────
    public class StatsResponse
    {
        public int TotalPredictions { get; set; }
        public int TotalDefectueux { get; set; }
        public float TauxDefaut { get; set; }
        public string NiveauRisqueMoyen { get; set; } = string.Empty;
        public Dictionary<string, int> RepartitionRisque { get; set; } = new();
        public Dictionary<string, int> RepartitionTypeDefaut { get; set; } = new();
        public List<EvolutionSemaine> EvolutionHebdo { get; set; } = new();
    }

    public class EvolutionSemaine
    {
        public string Semaine { get; set; } = string.Empty;
        public int TotalControles { get; set; }
        public int NbDefectueux { get; set; }
    }
}