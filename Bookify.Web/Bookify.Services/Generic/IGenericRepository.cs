using Bookify.Data.Data;
using Bookify.Services.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Bookify.Services.Generic
{
    public interface IGenericRepository<T> where T : class
    {
        Task<ResponseHelper<IEnumerable<T>>> FindAll();
        Task<ResponseHelper<IEnumerable<T>>> FindAll(Expression<Func<T, bool>> predicate);
        Task<ResponseHelper<T>> Find(Expression<Func<T, bool>> predicate);
        Task<ResponseHelper> Add(T entity);
        Task<ResponseHelper> Add(IEnumerable<T> entities);
        Task<ResponseHelper> Update(T entity);
        Task<ResponseHelper> Update(IEnumerable<T> entities);
        Task<ResponseHelper> Delete(T entity);
        Task<ResponseHelper> Delete(IEnumerable<T> entity);
    }
}
