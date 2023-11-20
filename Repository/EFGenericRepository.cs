using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Repository
{
    public class EFGenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : class
    {
        DbContext context;
        DbSet<TEntity> dbSet;
        public EFGenericRepository(DbContext context)
        {
            this.context = context;
            dbSet = context.Set<TEntity>();
        }
        public void Add(TEntity entity)
        {
            dbSet.Add(entity);
            context.SaveChanges();
        } 
        public async Task AddAsync(TEntity entity)
        {
            await dbSet.AddAsync(entity);
            await context.SaveChangesAsync();
        }

        public IEnumerable<TEntity> FindAll(Expression<Func<TEntity, bool>> expression)
        {
            return dbSet.Where(expression).ToList();
        }

        public TEntity FindById(params object[] id)
        {
            return dbSet.Find(id);
        }
        public async Task<IEnumerable<TEntity>> FindByIdAsync(params object[] id)
        {
            return (IEnumerable<TEntity>)await dbSet.FindAsync(id);
        }

        public IEnumerable<TEntity> GetAll()
        {
            return dbSet.ToList();
        }

        public void Remove(TEntity entity)
        {
            dbSet.Remove(entity);
            context.SaveChanges();
        }

        public async Task RemoveAsync(TEntity entity)
        {
            dbSet.Remove(entity);
            await context.SaveChangesAsync();
        }

        public void Update(TEntity entity)
        {
            context.Entry(entity).State = EntityState.Modified;
            context.SaveChanges();
        }
        public async Task  UpdateAsync(TEntity entity)
        {
            context.Entry(entity).State = EntityState.Modified;
            await context.SaveChangesAsync();
        }
    }
}