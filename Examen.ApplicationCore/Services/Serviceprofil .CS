using System;
using System.Collections.Generic;
using System.Linq;
using AM.ApplicationCore.Interfaces;
using Examen.ApplicationCore.Domain;
using Examen.ApplicationCore.DTOs;
using Examen.ApplicationCore.Entities;
using Examen.ApplicationCore.Interfaces;
using Examen.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;

namespace Examen.ApplicationCore.Services
{
    public class ServiceProfil : IServiceProfil
    {
        private readonly IUnitOfWork _unitOfWork;

        public ServiceProfil(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

     
        private IQueryable<Profil> QueryComplet()
        {
            return _unitOfWork.Repository<Profil>()
                .Query()
                .Include(p => p.ProfilMenus).ThenInclude(pm => pm.Menu)
                .Include(p => p.ProfilFonctionDroits).ThenInclude(pf => pf.Menu)
                .Include(p => p.Utilisateurs);
        }

        public List<ProfilDto> GetAll()
        {
            return QueryComplet().Select(p => ToDto(p)).ToList();
        }

        public ProfilDto? GetById(int id)
        {
            var profil = QueryComplet().FirstOrDefault(p => p.Id == id);
            return profil == null ? null : ToDto(profil);
        }

        public ProfilDto Create(CreateProfilRequest request)
        {
            ArgumentNullException.ThrowIfNull(request);

            var profil = new Profil
            {
                Nom = request.Nom,
                Description = request.Description
            };

            _unitOfWork.Repository<Profil>().Add(profil);
            Commit();

            return ToDto(profil);
        }

        public ProfilDto? Update(int id, CreateProfilRequest request)
        {
            ArgumentNullException.ThrowIfNull(request);

            var profil = _unitOfWork.Repository<Profil>().Query().FirstOrDefault(p => p.Id == id);
            if (profil == null) return null;

            profil.Nom = request.Nom;
            profil.Description = request.Description;

            _unitOfWork.Repository<Profil>().Update(profil);
            Commit();

            return ToDto(profil);
        }

        public bool Delete(int id)
        {
            var profil = _unitOfWork.Repository<Profil>().Query().FirstOrDefault(p => p.Id == id);
            if (profil == null) return false;

            _unitOfWork.Repository<Profil>().Delete(profil);
            Commit();
            return true;
        }

        public void Commit()
        {
            _unitOfWork.Save();
        }

        // ───────────────────────────────────────────────
        // Méthodes spécifiques
        // ───────────────────────────────────────────────
        public bool UpdateMenus(int profilId, List<ProfilMenuRequest> menus)
        {
            var profil = _unitOfWork.Repository<Profil>()
                .Query()
                .Include(p => p.ProfilMenus)
                .FirstOrDefault(p => p.Id == profilId);

            if (profil == null) return false;

            profil.ProfilMenus.Clear();
            foreach (var m in menus)
                profil.ProfilMenus.Add(new ProfilMenu { ProfilId = profilId, MenuId = m.MenuId, Visible = m.Visible });

            _unitOfWork.Repository<Profil>().Update(profil);
            Commit();
            return true;
        }

        public bool UpdateDroits(int profilId, List<ProfilFonctionDroitRequest> droits)
        {
            var profil = _unitOfWork.Repository<Profil>()
                .Query()
                .Include(p => p.ProfilFonctionDroits)
                .FirstOrDefault(p => p.Id == profilId);

            if (profil == null) return false;

            profil.ProfilFonctionDroits.Clear();
            foreach (var f in droits)
                profil.ProfilFonctionDroits.Add(new ProfilFonctionDroit
                {
                    ProfilId = profilId,
                    MenuId = f.MenuId,
                    Lecture = f.Lecture,
                    Creation = f.Creation,
                    Modification = f.Modification,
                    Suppression = f.Suppression
                });

            _unitOfWork.Repository<Profil>().Update(profil);
            Commit();
            return true;
        }

        private static ProfilDto ToDto(Profil p) => new()
        {
            Id = p.Id,
            Nom = p.Nom,
            Description = p.Description,
            NbUtilisateurs = p.Utilisateurs?.Count ?? 0,
            Menus = p.ProfilMenus?.Select(pm => new ProfilMenuDto
            {
                MenuId = pm.MenuId,
                NomMenu = pm.Menu?.Nom ?? "",
                Visible = pm.Visible
            }).ToList() ?? new(),
            FonctionDroits = p.ProfilFonctionDroits?.Select(pf => new ProfilFonctionDroitDto
            {
                MenuId = pf.MenuId,
                NomFonction = pf.Menu?.Nom ?? "",
                Lecture = pf.Lecture,
                Creation = pf.Creation,
                Modification = pf.Modification,
                Suppression = pf.Suppression
            }).ToList() ?? new()
        };
    }
}