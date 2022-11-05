using System;
using System.Collections.Generic;

namespace CleanBudget.Services.Repositories
{
    public interface IRepository<T>
    {
        T GetById(Guid id);
        IEnumerable<T> GetAll();
        void Create(T item);
        void Delete(T item);
        void Update(T item);
    }
}
