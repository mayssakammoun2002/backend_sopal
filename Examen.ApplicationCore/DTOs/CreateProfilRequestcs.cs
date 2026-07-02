namespace Examen.Infrastructure.Services
{
    public class CreateProfilRequest
    {
        public string Nom { get; set; } = string.Empty;
        public string? Description { get; set; }
    }
}