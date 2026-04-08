namespace Examen.ApplicationCore.DTOs
{
    public class ProduitDTO
    {
        public string CodeArticle { get; set; }
        public string NomProduit { get; set; }
        public string Designation { get; set; }
        public int TailleEchantillonnage { get; set; }
        public List<int> TypeDefautIds { get; set; } = new List<int>();
    }
}