using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Examen.ApplicationCore.Interfaces
{
    public interface IGenericRepository<TEntity> where TEntity : class
    {
        void Add(TEntity entity);
        void Update(TEntity entity);
        void Delete(TEntity entity);
        void Delete(Expression<Func<TEntity, bool>> where);

        TEntity? GetById(params object[] keyValues);
        TEntity? Get(Expression<Func<TEntity, bool>> where);

        IEnumerable<TEntity> GetAll();
        IEnumerable<TEntity> GetMany(Expression<Func<TEntity, bool>> where);
        IEnumerable<TEntity> Find(Expression<Func<TEntity, bool>> expression);

        bool Exists(object id);
        bool Exists(Expression<Func<TEntity, bool>> where);
        int Count(Expression<Func<TEntity, bool>> where);
    }
}