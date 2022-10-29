using System;
using System.Collections.Generic;

namespace CleanBudget.Services.Repositories
{
    public interface IRepository<T>
    {
        T GetById(string id);
        ICollection<T> GetAll();
        void Create(T item);
        void Delete(T item);
        void Update(T item);
    }
}
