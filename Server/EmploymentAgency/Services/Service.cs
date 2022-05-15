using System.Reflection;
using EmploymentAgency.Models;
using Microsoft.EntityFrameworkCore;

namespace EmploymentAgency.Services;

public class Service<T>
    where T : class, IIdentifiable
{
    private readonly EmploymentAgencyContext _context;
    private readonly Func<EmploymentAgencyContext, DbSet<T>> _getEntities;

    public Service(
        EmploymentAgencyContext context,
        Func<EmploymentAgencyContext, DbSet<T>> getEntities)
    {
        _context = context;
        _getEntities = getEntities;
    }

    public async Task CreateAsync(T entity)
    {
        entity.Id = default;
        await _getEntities(_context).AddAsync(entity);
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<T>> ReadAsync(int page, int pageSize)
    {
        return await _getEntities(_context)
            .OrderBy(x => x.Id)
            .Skip(page * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<T?> ReadAsync(int id)
    {
        return await _getEntities(_context).FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<IEnumerable<T>> ReadAsync(int page, int pageSize, string pattern)
    {
        var entities = await ReadAsync(page, pageSize);
        return await Task.Run(() =>
            entities.Where(e => AnyMemberContains(pattern, e)).ToList());
    }

    public async Task<bool> UpdateAsync(int id, T entity)
    {
        return await TryFindAsync(id, toUpdate =>
        {
            entity.Id = id;
            _context.Entry(toUpdate).CurrentValues.SetValues(entity);
        });
    }

    public async Task<bool> DeleteAsync(int id)
    {
        return await TryFindAsync(id, entity => _getEntities(_context).Remove(entity));
    }

    private bool AnyMemberContains(string substr, object entity)
    {
        return entity.GetType()
            .GetProperties()
            .Where(p => !p.Name.EndsWith("Id") && !p.Name.EndsWith("LazyLoader"))
            .Any(p =>
            {
                object? value = p.GetValue(entity);

                Console.WriteLine(
                    p.Name + " " +
                    p.GetValue(entity)?.ToString() + " " +
                    _context.Model.FindRuntimeEntityType(value.GetType()));

                if (value is IIdentifiable)
                {
                    return AnyMemberContains(substr, value);
                }

                return value?.ToString()?
                    .Contains(substr, StringComparison.OrdinalIgnoreCase) ?? false;
            });
    }

    private async Task<bool> TryFindAsync(int id, Action<T> callback)
    {
        var entity = await ReadAsync(id);
        if (entity is null)
        {
            return false;
        }

        callback(entity);

        await _context.SaveChangesAsync();
        return true;
    }
}
