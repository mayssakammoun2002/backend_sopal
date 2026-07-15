using System.Security.Cryptography;
using System.Text;
using BCrypt.Net;
using Examen.ApplicationCore.Domain;
using Examen.ApplicationCore.DTOs;
using Examen.ApplicationCore.DTOs.Common;
using Examen.ApplicationCore.Extensions;
using Examen.ApplicationCore.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Examen.ApplicationCore.Services
{
    public class ServiceUtilisateur : IServiceUtilisateur
    {
        private readonly IUnitOfWork _unitOfWork;

        public ServiceUtilisateur(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        public IEnumerable<Utilisateur> GetAll() =>
            _unitOfWork.Repository<Utilisateur>()
                .Query()
                .Include(u => u.Profil)
                .ToList();

        public Utilisateur? GetById(int id) =>
            _unitOfWork.Repository<Utilisateur>()
                .Query()
                .Include(u => u.Profil)
                .FirstOrDefault(u => u.Id == id);

        public void Add(Utilisateur user) => _unitOfWork.Repository<Utilisateur>().Add(user);
        public void Update(Utilisateur user) => _unitOfWork.Repository<Utilisateur>().Update(user);
        public void Delete(Utilisateur user) => _unitOfWork.Repository<Utilisateur>().Delete(user);

        public void DeleteById(int id)
        {
            var user = GetById(id);
            if (user != null) Delete(user);
        }

        public void Commit() => _unitOfWork.Save();

        // ------------------ Auth ------------------
        public Utilisateur? GetByEmail(string email)
        {
            return _unitOfWork.Repository<Utilisateur>()
                .Query()
                .Include(u => u.Profil) // ✅ crucial pour EstAdmin
                .FirstOrDefault(u => u.Email.ToLower() == email.ToLower());
        }

        public bool VerifyPassword(Utilisateur user, string password)
        {
            return BCrypt.Net.BCrypt.Verify(password, user.Password);
        }

        public bool VerifyPassword(string password, string hash)
        {
            return BCrypt.Net.BCrypt.Verify(password, hash);
        }

        public string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        // ✅ Utilisée par le contrôleur Signin
        public Task<Utilisateur?> Authenticate(string email, string password)
        {
            var user = GetByEmail(email);
            if (user == null || !user.Actif)
                return Task.FromResult<Utilisateur?>(null);
            if (!VerifyPassword(user, password))
                return Task.FromResult<Utilisateur?>(null);
            return Task.FromResult<Utilisateur?>(user);
        }

        // ------------------ Pagination ------------------
        public async Task<PaginatedResult<UtilisateurDTO>> GetUtilisateursPaginesAsync(PaginationParams paginationParams)
        {
            var query = _unitOfWork.Repository<Utilisateur>()
                .Query()
                .AsNoTracking();

            if (!string.IsNullOrWhiteSpace(paginationParams.Recherche))
            {
                var recherche = paginationParams.Recherche.ToLower();
                query = query.Where(u =>
                    u.FirstName.ToLower().Contains(recherche) ||
                    u.LastName.ToLower().Contains(recherche) ||
                    u.Email.ToLower().Contains(recherche));
            }

            query = query.OrderBy(u => u.LastName).ThenBy(u => u.FirstName);

            var dtoQuery = query.Select(u => new UtilisateurDTO
            {
                Id = u.Id,
                FirstName = u.FirstName,
                LastName = u.LastName,
                Email = u.Email,
                ProfilId = u.ProfilId,
                Actif = u.Actif
            });

            return await dtoQuery.ToPaginatedResultAsync(
                paginationParams.PageNumber,
                paginationParams.PageSize);
        }
    }
}