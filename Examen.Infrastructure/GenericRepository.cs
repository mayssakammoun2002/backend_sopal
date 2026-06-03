using AM.ApplicationCore.Interfaces;
using Examen.ApplicationCore.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace AM.Infrastructure
{
    public class GenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : class
    {
        private readonly DbContext _context;
        private readonly DbSet<TEntity> _dbSet;

        public GenericRepository(DbContext context)
        {
            _context = context;
            _dbSet = _context.Set<TEntity>();
        }

        public void Add(TEntity entity) => _dbSet.Add(entity);
        public void Update(TEntity entity) => _dbSet.Update(entity);
        public void Delete(TEntity entity) => _dbSet.Remove(entity);

        public void Delete(Expression<Func<TEntity, bool>> where)
        {
            var entities = _dbSet.Where(where).ToList();
            _dbSet.RemoveRange(entities);
        }

        // ✅ TEntity? — aligné avec l'interface et le retour réel de Find()
        public TEntity? GetById(params object[] keyValues) => _dbSet.Find(keyValues);

        // ✅ TEntity? — FirstOrDefault peut retourner null
        public TEntity? Get(Expression<Func<TEntity, bool>> where) => _dbSet.FirstOrDefault(where);

        public IEnumerable<TEntity> GetAll() => _dbSet.ToList();
        public IEnumerable<TEntity> GetMany(Expression<Func<TEntity, bool>> where) => _dbSet.Where(where).ToList();
        public IEnumerable<TEntity> Find(Expression<Func<TEntity, bool>> expression) => _dbSet.Where(expression).ToList();

        public bool Exists(object id)
        {
            if (id == null) return false;
            return _dbSet.Find(id) != null;
        }

        // ✅ Surcharge manquante
        public bool Exists(Expression<Func<TEntity, bool>> where) => _dbSet.Any(where);

        // ✅ Méthode manquante
        public int Count(Expression<Func<TEntity, bool>> where) => _dbSet.Count(where);
    }
}