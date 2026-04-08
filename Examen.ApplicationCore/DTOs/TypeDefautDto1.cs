using Microsoft.AspNetCore.Http;

namespace Examen.ApplicationCore.DTO
{
    public class TypeDefautDto
    {
        public string NomDefaut { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Solution { get; set; } = string.Empty;
        public int Frequence { get; set; }
        public string CodeArticle { get; set; } = string.Empty;

        public IFormFile? ImageFile { get; set; }
    }
}