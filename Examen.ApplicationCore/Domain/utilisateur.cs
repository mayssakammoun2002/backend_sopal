using System.ComponentModel.DataAnnotations;
using System.Data;

namespace Examen.ApplicationCore.Domain
{
    public class Utilisateur
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Le prénom est obligatoire")]
        [MinLength(3, ErrorMessage = "Minimum 3 caractères")]
        [MaxLength(25, ErrorMessage = "Maximum 25 caractères")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Le nom est obligatoire")]
        [MinLength(3)]
        [MaxLength(25)]
        public string LastName { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }  
        [Required]
        public Role Role { get; set; }

        [Required]
        public string Password { get; set; }
        public bool Actif { get; set; }
        //  Relation bidirectionnelle
        public ICollection<ResultatControle> ResultatControles { get; set; }
    }
}