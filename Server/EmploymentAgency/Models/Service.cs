using Microsoft.EntityFrameworkCore;

namespace EmploymentAgency.Models;

public class Service<T>
    where T : class, IIDentifiable
{
    private readonly Settings _settings;
    private readonly Func<EmploymentAgencyContext, DbSet<T>> _getEntities;

    public Service(Settings settings, Func<EmploymentAgencyContext, DbSet<T>> getEntities)
    {
        _settings = settings;
        _getEntities = getEntities;
    }

    public async Task CreateAsync(T entity)
    {
        await InContextAsync(async context => await CreateAsync(entity, context));
    }

    public async Task<T?> ReadAsync(int id)
    {
        return await InContextAsync(async context => await ReadAsync(id, context));
    }

    public async Task<IEnumerable<T>> ReadAsync()
    {
        return await InContextAsync(async context => await _getEntities(context).ToListAsync());
    }

    public async Task<bool> UpdateAsync(int id, T entity)
    {
        return await InContextAsync(async context => await UpdateAsync(id, entity, context));
    }

    public async Task<bool> DeleteAsync(int id)
    {
        return await InContextAsync(async context => await DeleteAsync(id, context));
    }

    private async Task<U> InContextAsync<U>(Func<EmploymentAgencyContext, Task<U>> callback)
    {
        using var context = new EmploymentAgencyContext(_settings);
        U result = await callback(context);
        await context.SaveChangesAsync();
        return result;
    }

    private async Task<Task<T>> CreateAsync(T entity, EmploymentAgencyContext context)
    {
        entity.Id = default;
        await _getEntities(context).AddAsync(entity);
        return null!;
    }

    private async Task<T?> ReadAsync(int id, EmploymentAgencyContext context)
    {
        return await _getEntities(context).FirstOrDefaultAsync(x => x.Id == id);
    }

    private async Task<bool> UpdateAsync(int id, T entity, EmploymentAgencyContext context)
    {
        var toUpdate = await ReadAsync(id, context);
        if (toUpdate is null)
        {
            return false;
        }

        entity.Id = id;
        context.Entry(toUpdate).CurrentValues.SetValues(entity);

        return true;
    }

    private async Task<bool> DeleteAsync(int id, EmploymentAgencyContext context)
    {
        var entity = await ReadAsync(id, context);
        if (entity is null)
        {
            return false;
        }

        _getEntities(context).Remove(entity);

        context.ChangeTracker.DetectChanges();
        Console.WriteLine(context.ChangeTracker.DebugView.ShortView);

        return true;
    }
}
