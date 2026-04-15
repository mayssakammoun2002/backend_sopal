using System.Security.Cryptography;
using System.Text;
using BCrypt.Net;
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
            return BCrypt.Net.BCrypt.Verify(password, user.Password);
        }
        public string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }
    }
}