using DataAccessLayer.Models;

namespace DataAccessLayer.Repository;

public interface ICounterRepository : IBaseRepository<OrderItems>
{
    public Task<int> AddCounter(int up, int down);
}
