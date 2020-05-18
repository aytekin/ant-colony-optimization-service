using Aco.Entity.Abstract;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Aco.Core.DataAccessLayer
{
    public interface IEntityRepository<T> where T : class, IEntity, new()
    {
        T Get(Expression<Func<T, bool>> filter);
        IList<T> GetList(Expression<Func<T, bool>> filter = null);
        T Add(T entity);
        void Update(T entity);
        void Delete(T entity);
    }
}
