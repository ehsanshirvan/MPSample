using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace MPSample.Domain.Common
{
    public interface IRepository<T>
    {
        IEnumerable<T> GetAll();
        T GetById(int id);
        void Add(T item);
        void AddRange(List<T> items);
        void DeleteById(int id);
        void Update(T item);

        IEnumerable<T> GetBy(Func<T, bool> whereClause);
        

    }
}
