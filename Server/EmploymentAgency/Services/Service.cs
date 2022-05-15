using System.Diagnostics;
using EmploymentAgency.Models;
using Microsoft.EntityFrameworkCore;

namespace EmploymentAgency.Services;

public class Service<T>
    where T : class, IIDentifiable
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
        return entities.Where(e => AnyMemberContains(pattern, e));
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

    private bool AnyMemberContains(string substr, T e)
    {
        return _context.Entry(e).Members
            .Any(m =>
            {
                Trace.Assert(!m.Metadata.Name.Contains("oading"));
                return m.CurrentValue?
                        .ToString()?
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
