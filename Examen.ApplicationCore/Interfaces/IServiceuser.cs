using Examen.ApplicationCore.Domain;

namespace Examen.ApplicationCore.Interfaces
{
    public interface IServiceUtilisateur
    {
        IEnumerable<Utilisateur> GetAll();
        Utilisateur? GetById(int id);
        void Add(Utilisateur user);
        void Update(Utilisateur user);
        void Delete(Utilisateur user);
        void DeleteById(int id);
        void Commit();

        Utilisateur? GetByEmail(string email);
        bool VerifyPassword(Utilisateur user, string password);
        string HashPassword(string password);
    }
}