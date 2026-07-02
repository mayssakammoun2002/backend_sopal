namespace Examen.ApplicationCore.DTOs
{
    public class ProfilMenuDto
    {
        public int MenuId { get; set; }
        public string NomMenu { get; set; } = string.Empty;
        public bool Visible { get; set; }
    }
}