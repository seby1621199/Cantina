using DataAccessLayer.Models;
using Microsoft.EntityFrameworkCore;

namespace DataAccessLayer.Repository;

public abstract class BaseRepository<T> : IBaseRepository<T> where T : class, IEntity
{
    protected readonly CantinaContext _context;
    public BaseRepository(CantinaContext context)
    {
        _context = context;
    }

    public async Task<int> Add(T entity)
    {
        await _context.AddAsync(entity);
        await _context.SaveChangesAsync();
        return entity.Id;
    }

    public async Task Update(T entity)
    {
        _context.Update(entity);
        await _context.SaveChangesAsync();
    }

    public async Task Update(int id, T entity)
    {
        var existingEntity = await GetById(id);
        if (existingEntity != null)
        {
            entity.Id = existingEntity.Id;
            _context.Entry(existingEntity).CurrentValues.SetValues(entity);


            await _context.SaveChangesAsync();
        }
        else
        {
            throw new KeyNotFoundException($"Entity with ID {id} not found.");
        }

    }


    public async Task<List<T>> GetAll()
    {
        return await _context.Set<T>().ToListAsync();
    }

    public async Task<T?> GetById(int id)
    {
        return await _context.Set<T>().SingleOrDefaultAsync(x => x.Id == id);
    }

    public async Task Delete(int id)
    {
        var entity = await GetById(id);
        if (entity != null)
        {
            _context.Remove(entity);
            await _context.SaveChangesAsync();
        }
        else
        {
            throw new KeyNotFoundException($"Entity with ID {id} not found.");
        }
    }
}