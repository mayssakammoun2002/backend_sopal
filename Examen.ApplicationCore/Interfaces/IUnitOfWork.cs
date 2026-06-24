using System;
using System.Threading.Tasks;
using AM.ApplicationCore.Interfaces;

namespace Examen.ApplicationCore.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        void Commit();
        IGenericRepository<TEntity> Repository<TEntity>() where TEntity : class;
        void Save();
        Task<int> SaveAsync();  
   
    }
}