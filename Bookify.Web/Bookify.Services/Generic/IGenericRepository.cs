using Bookify.Data.Data;
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
        Task<Response<IEnumerable<T>>> FindAll();
        Task<Response<IEnumerable<T>>> FindAll(Expression<Func<T, bool>> predicate);
        Task<Response<T>> Find(Expression<Func<T, bool>> predicate);
        Task<Response> Add(T entity);
        Task<Response> Add(IEnumerable<T> entities);
        Task<Response> Update(T entity);
        Task<Response> Update(IEnumerable<T> entities);
        Task<Response> Delete(T entity);
        Task<Response> Delete(IEnumerable<T> entity);
    }
}
