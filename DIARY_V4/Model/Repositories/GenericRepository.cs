using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DIARY_V4.Model
{
    public class GenericRepository<T> : IRepository<T> where T : class
    {
        private readonly BaseDbContext _dbcontext;

        private IDbSet<T> _dbSet => _dbcontext.Set<T>();
        public IQueryable<T> Entities => _dbSet;

        public GenericRepository(BaseDbContext dbContext)
        {
            _dbcontext = dbContext;
        }

        public void Remove(T entity)
        {
            _dbSet.Remove(entity);
        }

        public void Add(T entity)
        {
            _dbSet.Add(entity);
        }
    }
}
