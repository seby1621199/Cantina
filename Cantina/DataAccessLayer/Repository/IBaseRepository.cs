using DataAccessLayer.Models;

namespace DataAccessLayer.Repository
{
    public interface IBaseRepository<T> where T : IEntity
    {
        Task<int> Add(T entity);
        Task<List<T>> GetAll();
        Task<T?> GetById(int id);
        Task Update(T entity);
        Task Delete(int id);
        Task Update(int id, T entity);
    }
}
