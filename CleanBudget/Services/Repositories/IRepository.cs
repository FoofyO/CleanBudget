using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanBudget.Services.Repositories
{
    public interface IRepository<T>
    {
        T GetById(int id);
        ICollection<T> GetAll();
        void Create(T item);
        void Delete(T item);
        void Update(T item);
    }
}
