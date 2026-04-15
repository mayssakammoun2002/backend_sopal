using Examen.ApplicationCore.Domain;

namespace Examen.ApplicationCore.Interfaces
{
    public interface IUtilisateurRepository
    {
        IEnumerable<Utilisateur> GetAll();
        Utilisateur GetById(int id);
        void Add(Utilisateur utilisateur);
        void Delete(int id);
    }
}