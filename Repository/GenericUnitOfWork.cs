using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public class GenericUnitOfWork : IDisposable
    {
        readonly DbContext _dbContext;
        public GenericUnitOfWork(DbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public Dictionary<Type, object> repositories = new();
        public IGenericRepository<TEntity> Repository<TEntity>() where TEntity : class
        {
            //if (repositories.ContainsKey(typeof(TEntity)))
            if (repositories.Keys.Contains(typeof(TEntity)))
            {
                return repositories[typeof(TEntity)] as IGenericRepository<TEntity>;
            }
            IGenericRepository<TEntity> repo = new EFGenericRepository<TEntity>(_dbContext);
            repositories.Add(typeof(TEntity), repo);
            return repo;
        }
        public void SaveChanges()
        {
            _dbContext.SaveChanges();
        }
        public void Dispose()
        {
            _dbContext.Dispose();
        }
    }
}
