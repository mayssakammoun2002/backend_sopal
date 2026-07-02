using System.Collections.Generic;
using Examen.ApplicationCore.DTOs;
using Examen.Infrastructure.Services;

namespace Examen.ApplicationCore.Interfaces
{
    public interface IServiceProfil
    {
        List<ProfilDto> GetAll();
        ProfilDto? GetById(int id);
        ProfilDto Create(CreateProfilRequest request);
        ProfilDto? Update(int id, CreateProfilRequest request);
        bool Delete(int id);
        void Commit();

        bool UpdateMenus(int profilId, List<ProfilMenuRequest> menus);
        bool UpdateDroits(int profilId, List<ProfilFonctionDroitRequest> droits);
    }
}