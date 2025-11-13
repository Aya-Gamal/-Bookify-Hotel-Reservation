using Azure;
using Bookify.Data.Data;
using Bookify.Services.Helpers;
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
        private readonly IDbContextFactory<AppDbContext> _contextFactory;
        AppDbContext dbContext;
        DbSet<T> dbSet;

        public GenericRepository(AppDbContext context)
        {
            dbContext = context;
            dbSet = dbContext.Set<T>();
        }

        public async Task<ResponseHelper<int>> SaveChangesAsync( CancellationToken cancellationToken = default)
        {
            try
            {
                int result = await dbContext.SaveChangesAsync(cancellationToken);
                return ResponseHelper<int>.Ok(result);
            }
            catch (DbUpdateConcurrencyException ucex)
            {
                return ResponseHelper<int>.Fail($"{ucex.Message}\n{ucex.InnerException.Message}");

            }
            catch (DbUpdateException uex)
            {
                return ResponseHelper<int>.Fail($"{uex.Message}\n{uex.InnerException.Message}");
            }
            catch (Exception ex)
            {
                return ResponseHelper<int>.Fail($"{ex.Message}\n{ex.InnerException.Message}");
            }
        }

        public async Task<ResponseHelper> Add(T entity)
        {
            await dbSet.AddAsync(entity);
            var result = await SaveChangesAsync();
            if (result.Error)
            {
                return ResponseHelper.Fail(result.Message);
            }
            return ResponseHelper.OK();
        }

        public async Task<ResponseHelper> Add(IEnumerable<T> entities)
        {
            await dbSet.AddRangeAsync(entities);
            var result = await SaveChangesAsync();
            if (result.Error)
            {
                return ResponseHelper.Fail(result.Message);
            }
            return ResponseHelper.OK();
        }

        public async Task<ResponseHelper> Delete(T entity)
        {
            dbSet.Remove(entity);
            var result = await SaveChangesAsync();
            if (result.Error)
            {
                return ResponseHelper.Fail(result.Message);
            }
            return ResponseHelper.OK();
        }

        public async Task<ResponseHelper> Delete(IEnumerable<T> entities)
        {
            dbSet.RemoveRange(entities);
            var result = await SaveChangesAsync();
            if (result.Error)
            {
                return ResponseHelper.Fail(result.Message);
            }
            return ResponseHelper.OK();
        }

        public async Task<ResponseHelper<T>> Find(Expression<Func<T, bool>> predicate)
        {
            var entity = await dbSet.SingleOrDefaultAsync(predicate);
            if (entity is null)
            {
                return ResponseHelper<T>.Fail("Item not found");
            }
            return ResponseHelper<T>.Ok(entity);
        }

        public async Task<ResponseHelper<IEnumerable<T>>> FindAll()
        {
            IEnumerable<T> entities = await dbSet.ToListAsync();
            if (!entities.Any())
            {
                return ResponseHelper<IEnumerable<T>>.Fail("No items found");
            }
            return ResponseHelper<IEnumerable<T>>.Ok(entities);
        }

        public async Task<ResponseHelper<IEnumerable<T>>> FindAll(Expression<Func<T, bool>> predicate)
        {
            var entities = await dbSet.Where(predicate).ToListAsync();
            if (!entities.Any())
            {
                return ResponseHelper<IEnumerable<T>>.Fail("No items found");
            }
            return ResponseHelper<IEnumerable<T>>.Ok(entities);
        }

        public async Task<ResponseHelper> Update(T entity)
        {
            dbSet.Update(entity);
            var result = await SaveChangesAsync();
            if (result.Error)
            {
                return ResponseHelper.Fail(result.Message);
            }
            return ResponseHelper.OK();
        }

        public async Task<ResponseHelper> Update(IEnumerable<T> entities)
        {
            dbSet.UpdateRange(entities);
            var result = await SaveChangesAsync();
            if (result.Error)
            {
                return ResponseHelper.Fail(result.Message);
            }
            return ResponseHelper.OK();
        }
    }
}
