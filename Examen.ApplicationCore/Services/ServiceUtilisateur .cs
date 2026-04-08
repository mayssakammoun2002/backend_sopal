using System.Security.Cryptography;
using System.Text;
using Examen.ApplicationCore.Domain;
using Examen.ApplicationCore.Interfaces;

namespace Examen.ApplicationCore.Services
{
    public class ServiceUtilisateur : IServiceUtilisateur
    {
        private readonly IUnitOfWork _unitOfWork;

        public ServiceUtilisateur(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        public IEnumerable<Utilisateur> GetAll() => _unitOfWork.Repository<Utilisateur>().GetAll();
        public Utilisateur? GetById(int id) => _unitOfWork.Repository<Utilisateur>().GetById(id);
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
                .GetAll()
                .FirstOrDefault(u => u.Email.ToLower() == email.ToLower());
        }

        public bool VerifyPassword(Utilisateur user, string password)
        {
            if (user == null || string.IsNullOrEmpty(user.Password)) return false;
            var hash = HashPassword(password);
            return hash == user.Password;
        }

        public string HashPassword(string password)
        {
            using var sha = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(password);
            var hashBytes = sha.ComputeHash(bytes);
            return Convert.ToBase64String(hashBytes);
        }
    }
}