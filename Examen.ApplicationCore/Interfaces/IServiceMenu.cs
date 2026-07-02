using System.Collections.Generic;
using System.Threading.Tasks;
using Examen.ApplicationCore.Entities;

namespace Examen.ApplicationCore.Interfaces
{
    public interface IServiceMenu
    {
        List<Menu> GetAll();
        Menu? GetById(int id);
        Menu Create(Menu menu);
        Menu? Update(int id, Menu updated);
        bool Delete(int id);
        void Commit();

        Task<List<Menu>> GetAllAsync();
        Task<Menu?> GetByIdAsync(int id);
        Task<Menu> CreateAsync(Menu menu);
        Task<Menu?> UpdateAsync(int id, Menu menu);
        Task<bool> DeleteAsync(int id);
    }
}