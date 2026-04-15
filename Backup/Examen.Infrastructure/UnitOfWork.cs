using AM.ApplicationCore.Interfaces;
using AM.Infrastructure;
using Examen.ApplicationCore.Interfaces;   // ← garde celui-là (ou AM si c'est vraiment l'interface utilisée)
using Examen.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Examen.Infrastructure
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ExamenDbContext _context;
        private readonly Dictionary<Type, object> _repositories = new();
        private bool _disposed;

        public UnitOfWork(ExamenDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public IGenericRepository<TEntity> Repository<TEntity>() where TEntity : class
        {
            var type = typeof(TEntity);
            if (!_repositories.TryGetValue(type, out var repo))
            {
                var repoType = typeof(GenericRepository<>).MakeGenericType(type);
                repo = Activator.CreateInstance(repoType, _context)
                    ?? throw new InvalidOperationException($"Impossible de créer le repository pour {type.Name}");
                _repositories[type] = repo;
            }
            return (IGenericRepository<TEntity>)repo;
        }

        // ────────────────────────────────────────────────
        // Méthode corrigée : void au lieu de int
        // ────────────────────────────────────────────────
        public void Save()
        {
            _context.SaveChanges();
        }

        public async Task<int> SaveAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                _context?.Dispose();
                _repositories.Clear();
                _disposed = true;
            }
        }

        public void Commit()
        {
            throw new NotImplementedException();
        }
    }
}