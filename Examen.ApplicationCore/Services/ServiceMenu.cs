using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Examen.ApplicationCore.DTOs;
using Examen.ApplicationCore.Entities;
using Examen.ApplicationCore.Interfaces;
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

        public List<MenuUtilisateurDto> GetMenuPourAdmin()
        {
            var tousMenus = _unitOfWork.Repository<Menu>()
                .Query()
                .Where(m => m.Visible)
                .OrderBy(m => m.Rang)
                .ToList();

            const int RACINE = -1;

            var parMenuParent = tousMenus
                .Where(m => !m.EstFonction)
                .GroupBy(m => m.ParentId ?? RACINE)
                .ToDictionary(g => g.Key, g => g.ToList());

            List<MenuUtilisateurDto> ConstruireFonctions(int? parentId) =>
                tousMenus
                    .Where(m => m.EstFonction && m.ParentId == parentId)
                    .Select(f => new MenuUtilisateurDto
                    {
                        Id = f.Id,
                        Nom = f.Nom,
                        Icone = f.Icone,
                        Lien = f.Lien,
                        EstFonction = true,
                        Rang = f.Rang,
                        Droits = new DroitsDto
                        {
                            Lecture = true,
                            Creation = true,
                            Modification = true,
                            Suppression = true
                        }
                    }).ToList();

            List<MenuUtilisateurDto> ConstruireArbre(int parentId)
            {
                if (!parMenuParent.TryGetValue(parentId, out var enfants))
                    return new List<MenuUtilisateurDto>();

                return enfants.Select(m => new MenuUtilisateurDto
                {
                    Id = m.Id,
                    Nom = m.Nom,
                    Icone = m.Icone,
                    Lien = m.Lien,
                    EstFonction = false,
                    Rang = m.Rang,
                    Enfants = ConstruireArbre(m.Id)
                        .Concat(ConstruireFonctions(m.Id))
                        .OrderBy(x => x.Rang)
                        .ToList()
                }).ToList();
            }

            return ConstruireArbre(RACINE)
                .Concat(ConstruireFonctions(null)) // ✅ inclut les fonctions racine (sans parent)
                .OrderBy(x => x.Rang)
                .ToList();
        }

        public List<MenuUtilisateurDto> GetMenuPourProfil(int profilId)
        {
            var tousMenus = _unitOfWork.Repository<Menu>()
                .Query()
                .Where(m => m.Visible)
                .OrderBy(m => m.Rang)
                .ToList();

            var profilMenus = _unitOfWork.Repository<ProfilMenu>()
                .Query()
                .Where(pm => pm.ProfilId == profilId)
                .ToDictionary(pm => pm.MenuId, pm => pm.Visible);

            var droits = _unitOfWork.Repository<ProfilFonctionDroit>()
                .Query()
                .Where(d => d.ProfilId == profilId)
                .ToDictionary(d => d.MenuId);

            var menusAutorises = tousMenus
                .Where(m => profilMenus.TryGetValue(m.Id, out var visible) && visible)
                .ToList();

            const int RACINE = -1;

            var parMenuParent = menusAutorises
                .Where(m => !m.EstFonction)
                .GroupBy(m => m.ParentId ?? RACINE)
                .ToDictionary(g => g.Key, g => g.ToList());

            List<MenuUtilisateurDto> ConstruireFonctions(int? parentId) =>
                menusAutorises
                    .Where(m => m.EstFonction && m.ParentId == parentId)
                    .Select(f =>
                    {
                        droits.TryGetValue(f.Id, out var d);
                        return new MenuUtilisateurDto
                        {
                            Id = f.Id,
                            Nom = f.Nom,
                            Icone = f.Icone,
                            Lien = f.Lien,
                            EstFonction = true,
                            Rang = f.Rang,
                            Droits = new DroitsDto
                            {
                                Lecture = d?.Lecture ?? false,
                                Creation = d?.Creation ?? false,
                                Modification = d?.Modification ?? false,
                                Suppression = d?.Suppression ?? false
                            }
                        };
                    }).ToList();

            List<MenuUtilisateurDto> ConstruireArbre(int parentId)
            {
                if (!parMenuParent.TryGetValue(parentId, out var enfants))
                    return new List<MenuUtilisateurDto>();

                return enfants.Select(m => new MenuUtilisateurDto
                {
                    Id = m.Id,
                    Nom = m.Nom,
                    Icone = m.Icone,
                    Lien = m.Lien,
                    EstFonction = false,
                    Rang = m.Rang,
                    Enfants = ConstruireArbre(m.Id)
                        .Concat(ConstruireFonctions(m.Id))
                        .OrderBy(x => x.Rang)
                        .ToList()
                }).ToList();
            }

            return ConstruireArbre(RACINE)
                .Concat(ConstruireFonctions(null)) // ✅ inclut les fonctions racine (sans parent)
                .OrderBy(x => x.Rang)
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