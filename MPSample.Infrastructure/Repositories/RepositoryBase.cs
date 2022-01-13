using Microsoft.EntityFrameworkCore;
using MPSample.Domain.Common;
using MPSample.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace MPSample.Infrastructure.Repositories
{

    public class RepositoryBase<T> : IRepository<T> where T:BaseEntity
    { 
        private readonly DbSet<T> _dbSet;

        public RepositoryBase(DbContext dbContext)
        {
            _dbSet = dbContext.Set<T>();
        }
        public void Add(T item)
        {
            _dbSet.Add(item);
        }

        public void AddRange(List<T> items)
        {
            _dbSet.AddRange(items);
        }

        public void DeleteById(int id)
        {
            var itemToDelete = _dbSet.Find(id);
            _dbSet.Remove(itemToDelete);
        }

        public IEnumerable<T> GetAll()
        {
            return _dbSet.ToList();
        }

        public IEnumerable<T> GetBy(Func<T, bool> whereClause)
        {
            return _dbSet.Where(whereClause);
        }

        public T GetById(int id)
        {
            return _dbSet.FirstOrDefault(x=>x.Id == id);
        }

      

        public void Update(T item)
        {
            _dbSet.Update(item);
        }
    }
}
