using System.ComponentModel.DataAnnotations;

namespace Examen.ApplicationCore.DTOs
{
    public class UtilisateurDTO
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Le prénom est obligatoire")]
        [MinLength(3)]
        [MaxLength(25)]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Le nom est obligatoire")]
        [MinLength(3)]
        [MaxLength(25)]
        public string LastName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public int Role { get; set; }

        public int? ProfilId { get; set; }

        public bool Actif { get; set; }
        public string? Matricule { get; set; }
        public string Token { get; set; }
    }
}