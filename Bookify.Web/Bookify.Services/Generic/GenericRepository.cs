using Azure;
using Bookify.Data.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Bookify.Services.Generic
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        AppDbContext dbContext;
        DbSet<T> dbSet;

        public GenericRepository(AppDbContext context)
        {
            dbContext = context;
            dbSet = dbContext.Set<T>();
        }

        public async Task<Response<int>> SaveChangesAsync( CancellationToken cancellationToken = default)
        {
            try
            {
                int result = await dbContext.SaveChangesAsync(cancellationToken);
                return Response<int>.Ok(result);
            }
            catch (DbUpdateConcurrencyException ucex)
            {
                return Response<int>.Fail($"{ucex.Message}\n{ucex.InnerException.Message}");

            }
            catch (DbUpdateException uex)
            {
                return Response<int>.Fail($"{uex.Message}\n{uex.InnerException.Message}");
            }
            catch (Exception ex)
            {
                return Response<int>.Fail($"{ex.Message}\n{ex.InnerException.Message}");
            }
        }

        public async Task<Response> Add(T entity)
        {
            await dbSet.AddAsync(entity);
            await dbContext.SaveChangesAsync();
            return Response.OK();
        }

        public async Task<Response> Add(IEnumerable<T> entities)
        {
            await dbSet.AddRangeAsync(entities);
            await dbContext.SaveChangesAsync();
            return Response.OK();
        }

        public async Task<Response> Delete(T entity)
        {
            dbSet.Remove(entity);
            await dbContext.SaveChangesAsync();
            return Response.OK();
        }

        public async Task<Response> Delete(IEnumerable<T> entities)
        {
            dbSet.RemoveRange(entities);
            await dbContext.SaveChangesAsync();
            return Response.OK();
        }

        public async Task<Response<T>> Find(Expression<Func<T, bool>> predicate)
        {
            var entity = await dbSet.SingleOrDefaultAsync(predicate);
            if (entity is null)
            {
                return Response<T>.Fail("Item not found");
            }
            return Response<T>.Ok(entity);
        }

        public async Task<Response<IEnumerable<T>>> FindAll()
        {
            IEnumerable<T> entities = await dbSet.ToListAsync();
            if (!entities.Any())
            {
                return Response<IEnumerable<T>>.Fail("No items found");
            }
            return Response<IEnumerable<T>>.Ok(entities);
        }

        public async Task<Response<IEnumerable<T>>> FindAll(Expression<Func<T, bool>> predicate)
        {
            var entities = await dbSet.Where(predicate).ToListAsync();
            if (!entities.Any())
            {
                return Response<IEnumerable<T>>.Fail("No items found");
            }
            return Response<IEnumerable<T>>.Ok(entities);
        }

        public async Task<Response> Update(T entity)
        {
            dbSet.Update(entity);
            await dbContext.SaveChangesAsync();
            return Response.OK();
        }

        public async Task<Response> Update(IEnumerable<T> entities)
        {
            dbSet.UpdateRange(entities);
            await dbContext.SaveChangesAsync();
            return Response.OK();
        }
    }
}
