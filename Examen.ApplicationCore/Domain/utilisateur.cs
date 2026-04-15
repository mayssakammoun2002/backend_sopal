using System.ComponentModel.DataAnnotations;
using Examen.ApplicationCore.Domain;

namespace Examen.ApplicationCore.Domain
{
    public class Utilisateur
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        public string LastName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        public Role Role { get; set; }

        [Required]
        public string Password { get; set; } = string.Empty;

        public bool Actif { get; set; } = true;

        // Navigation
        public ICollection<ResultatControle> ResultatControles { get; set; }
            = new List<ResultatControle>();
    }
}