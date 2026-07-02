using System.ComponentModel.DataAnnotations;

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

        public ICollection<ResultatControle> ResultatControles { get; set; }
            = new List<ResultatControle>();
        public ICollection<Lot> Lots { get; set; } = new List<Lot>();
        public int? ProfilId { get; set; }
        public Profil? Profil { get; set; }
    }

}