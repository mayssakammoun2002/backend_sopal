using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AM.ApplicationCore.Interfaces;
using Examen.ApplicationCore.DTOs;
using Examen.ApplicationCore.Entities;
using Examen.ApplicationCore.Interfaces;
using Examen.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
namespace Examen.ApplicationCore.Services
{
    public class ServiceMenu : IServiceMenu
    {
        private readonly IUnitOfWork _unitOfWork;
        public ServiceMenu(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }
        public List<Menu> GetAll() => _unitOfWork.Repository<Menu>()
                .Query()
                .Include(m => m.Parent)
                .Include(m => m.Enfants)
                .Include(m => m.TypeFonction)
                .OrderBy(m => m.Rang)
                .ToList();
        public Menu? GetById(int id)
        {
            return _unitOfWork.Repository<Menu>()
                .Query()
                .Include(m => m.Parent)
                .Include(m => m.Enfants)
                .Include(m => m.TypeFonction)
                .FirstOrDefault(m => m.Id == id);
        }
        public List<MenuUtilisateurDto> GetMenuPourProfil(int profilId)
        {
            // Menus visibles pour ce profil (jonction ProfilMenu)
            var visibleIds = _unitOfWork.Repository<ProfilMenu>()
                .Query()
                .Where(pm => pm.ProfilId == profilId && pm.Visible)
                .Select(pm => pm.MenuId)
                .ToHashSet();

            if (visibleIds.Count == 0) return new List<MenuUtilisateurDto>();

            // Droits CRUD par menu-fonction pour ce profil
            var droitsParMenu = _unitOfWork.Repository<ProfilFonctionDroit>()
                .Query()
                .Where(d => d.ProfilId == profilId)
                .ToDictionary(d => d.MenuId, d => new DroitsDto
                {
                    Lecture = d.Lecture,
                    Creation = d.Creation,
                    Modification = d.Modification,
                    Suppression = d.Suppression
                });

            var tousLesMenus = _unitOfWork.Repository<Menu>()
                .Query()
                .OrderBy(m => m.Rang)
                .ToList();

            MenuUtilisateurDto? Construire(Menu m)
            {
                if (!visibleIds.Contains(m.Id)) return null;

                var enfants = tousLesMenus
                    .Where(e => e.ParentId == m.Id)
                    .Select(Construire)
                    .Where(e => e != null)
                    .Cast<MenuUtilisateurDto>()
                    .ToList();

                // Un conteneur (non-fonction) sans aucun enfant visible ne sert à rien
                if (!m.EstFonction && enfants.Count == 0) return null;

                return new MenuUtilisateurDto
                {
                    Id = m.Id,
                    Nom = m.Nom,
                    Icone = m.Icone,
                    Lien = m.Lien,
                    EstFonction = m.EstFonction,
                    Rang = m.Rang,
                    Droits = m.EstFonction && droitsParMenu.TryGetValue(m.Id, out var d) ? d : null,
                    Enfants = enfants
                };
            }

            return tousLesMenus
                .Where(m => m.ParentId == null)
                .Select(Construire)
                .Where(m => m != null)
                .Cast<MenuUtilisateurDto>()
                .ToList();
        }
        public Menu Create(Menu menu)
        {
            ArgumentNullException.ThrowIfNull(menu);
            _unitOfWork.Repository<Menu>().Add(menu);
            Commit();
            return menu;
        }
        public Menu? Update(int id, Menu updated)
        {
            ArgumentNullException.ThrowIfNull(updated);
            var existing = _unitOfWork.Repository<Menu>().GetById(id);
            if (existing == null) return null;
            existing.Nom = updated.Nom;
            existing.Icone = updated.Icone;
            existing.Lien = updated.Lien;
            existing.EstFonction = updated.EstFonction;
            existing.Visible = updated.Visible;
            existing.Rang = updated.Rang;
            existing.ParentId = updated.ParentId;
            existing.TypeFonctionId = updated.TypeFonctionId;
            _unitOfWork.Repository<Menu>().Update(existing);
            Commit();
            return existing;
        }
        public bool Delete(int id)
        {
            var menu = _unitOfWork.Repository<Menu>().GetById(id);
            if (menu == null) return false;
            _unitOfWork.Repository<Menu>().Delete(menu);
            Commit();
            return true;
        }
        public void Commit()
        {
            _unitOfWork.Save();
        }
        public Task<List<Menu>> GetAllAsync()
        {
            throw new NotImplementedException();
        }
        public Task<Menu?> GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }
        public Task<Menu> CreateAsync(Menu menu)
        {
            throw new NotImplementedException();
        }
        public Task<Menu?> UpdateAsync(int id, Menu menu)
        {
            throw new NotImplementedException();
        }
        public Task<bool> DeleteAsync(int id)
        {
            throw new NotImplementedException();
        }
    }
}