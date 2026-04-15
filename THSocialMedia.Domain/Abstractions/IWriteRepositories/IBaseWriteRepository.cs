using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Query;

namespace THSocialMedia.Domain.Abstractions.IRepositories
{
    public interface IBaseWriteRepository<T>
    {
        Task<T> GetByIdAsync(Guid id);
        //Task<IEnumerable<T>> GetAllAsync();
        void Add(T entity);
        void Update(T entity);
        void Remove(T entity);
        Task<IList<T>> GetAllAsync(Expression<Func<T, bool>> predicate = null,
            Func<IQueryable<T>,
            IOrderedQueryable<T>> orderBy = null,
            Func<IQueryable<T>, IIncludableQueryable<T,
             object>> include = null);

        Task<T> GetFirstOrDefault(
        Expression<Func<T, bool>> predicate,
        Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
        Func<IQueryable<T>, IIncludableQueryable<T, object>> include = null);
    }
}
