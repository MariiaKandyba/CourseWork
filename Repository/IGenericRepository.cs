using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public interface IGenericRepository<TEntity> where TEntity : class
    {
        //по id має бути
        void Add(TEntity entity);
        void Remove(TEntity entity);
        void Update(TEntity entity);
        IEnumerable<TEntity> GetAll();
        IEnumerable<TEntity> FindAll(Expression<Func<TEntity, bool>> expression);
        TEntity FindById(params object[] id);
    }
}
