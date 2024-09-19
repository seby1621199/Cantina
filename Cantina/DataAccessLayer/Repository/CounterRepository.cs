using DataAccessLayer.Models;

namespace DataAccessLayer.Repository;

public class CounterRepository : BaseRepository<OrderItems>, ICounterRepository
{
    public CounterRepository(CantinaContext context) : base(context)
    {
    }

    public async Task<int> AddCounter(int up, int down)
    {
        var counter = new Counter
        {
            Up = up,
            Down = down,
            Difference=up-down
        };
        await _context.AddAsync(counter);
        await _context.SaveChangesAsync();
        return counter.Id;
    }


}