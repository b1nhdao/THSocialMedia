using System.Linq.Expressions;

namespace THSocialMedia.Domain.Repositories
{
    public interface IBaseRepository<T>
    {
        Task<T> GetByIdAsync(Guid id);
        Task<IEnumerable<T>> GetAllAsync();
        void Add(T entity);
        void Update(T entity);
        void Delete(Guid id);
    }
}
