using Microsoft.AspNetCore.Http;

namespace Examen.ApplicationCore.DTO
{
    public class TypeDefautDto
    {
        public string NomDefaut { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        public string CauseProbable { get; set; } = string.Empty; // ✅ NEW

        public string Solution { get; set; } = string.Empty;
        public int Frequence { get; set; }

        public IFormFile? ImageFile { get; set; }
    }
}