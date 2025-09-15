using Services.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interfaces
{
    public interface ICrudRepository<T> where T: class
    {
        public T? Get(int id);

        public IQueryable<T> GetAll();

        public (ServiceResult<T>, int id) Add(T obj);

        public ServiceResult<T> Update(int id, T obj);
        public ServiceResult<bool> Delete(int id);
    }
}
