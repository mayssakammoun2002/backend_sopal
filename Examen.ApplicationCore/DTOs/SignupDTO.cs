namespace Examen.ApplicationCore.DTOs
{
    public class SignupDTO
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; } // on va utiliser email comme identifiant
        public string Password { get; set; } // mot de passe en clair venant du frontend
    }
}