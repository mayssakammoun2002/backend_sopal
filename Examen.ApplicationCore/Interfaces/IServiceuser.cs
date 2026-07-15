using System.Threading.Tasks;
using Examen.ApplicationCore.Domain;
using Examen.ApplicationCore.DTOs;
using Examen.ApplicationCore.DTOs.Common;

namespace Examen.ApplicationCore.Interfaces
{
    public interface IServiceUtilisateur
    {
        Task<Utilisateur?> Authenticate(string email, string password);
        Utilisateur? GetById(int id);
        IEnumerable<Utilisateur> GetAll();
        void Add(Utilisateur user);
        void Update(Utilisateur user);
        void DeleteById(int id);
        void Commit();
        Task<PaginatedResult<UtilisateurDTO>> GetUtilisateursPaginesAsync(PaginationParams paginationParams);
        string HashPassword(string password);
        bool VerifyPassword(string password, string hash);
    }
}