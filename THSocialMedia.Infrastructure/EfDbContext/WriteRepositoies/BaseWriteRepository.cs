using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using THSocialMedia.Domain.Abstractions.IRepositories;
using THSocialMedia.Domain.Entities;

namespace THSocialMedia.Infrastructure.EfDbContext.WriteRepositoies
{
    public abstract class BaseWriteRepository<TEntity>(WriteDbContext dbContext) : IBaseWriteRepository<TEntity>
    where TEntity : BaseEntity
    {

        private readonly DbSet<TEntity> _dbSet = dbContext.Set<TEntity>();
        protected readonly WriteDbContext DbContext = dbContext;

        private static readonly Func<WriteDbContext, Guid, Task<TEntity>> GetByIdCompiledAsync =
      EF.CompileAsyncQuery((WriteDbContext dbContext, Guid id) =>
          dbContext
              .Set<TEntity>()
              .AsNoTrackingWithIdentityResolution()
              .FirstOrDefault(entity => entity.Id.Equals(id)))!;


        public void Add(TEntity entity)
        {
            _dbSet.Add(entity);
        }

        public async Task<TEntity> GetByIdAsync(Guid id)

        => await GetByIdCompiledAsync(DbContext, id);


        public void Remove(TEntity entity)
        {
            _dbSet.Remove(entity);

        }

        public void Update(TEntity entity)
        {
            _dbSet.Update(entity);
        }


        #region IDisposable

        // To detect redundant calls.
        private bool _disposed;

        ~BaseWriteRepository() => Dispose(false);

        // Public implementation of Dispose pattern callable by consumers.
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        // Protected implementation of Dispose pattern.
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            // Dispose managed state (managed objects).
            if (disposing)
                DbContext.Dispose();

            _disposed = true;
        }

        public async Task<TEntity> GetFirstOrDefault(
            Expression<Func<TEntity, bool>> predicate,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null)
        {
            try
            {
                IQueryable<TEntity> query = DbContext.Set<TEntity>();

                if (include != null) query = include(query);

                query = query.Where(predicate);
                if (orderBy != null) query = orderBy(query);

                var a = await query.FirstOrDefaultAsync();
                return a;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<IList<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>> predicate = null,
            Func<IQueryable<TEntity>,
            IOrderedQueryable<TEntity>> orderBy = null,
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity,
             object>> include = null)
        {
            try
            {
                IQueryable<TEntity> query = DbContext.Set<TEntity>().AsNoTracking();

                if (include != null) query = include(query);

                if (predicate != null) query = query.Where(predicate);

                if (orderBy != null) query = orderBy(query);

                return await query.ToListAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion
    }
}
