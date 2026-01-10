using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using FinovaERP.Application.Interfaces;
using FinovaERP.Infrastructure.Data;
namespace FinovaERP.Infrastructure.Repositories
{
/// <summary>
/// Generic repository implementation with Entity Framework
/// </summary>
/// <typeparam name="T">Entity type</typeparam>
public class Repository<T> : IRepository<T> where T : class
{
protected readonly FinovaDbContext _context;
protected readonly DbSet<T> _dbSet;
public Repository(FinovaDbContext context)
{
_context = context;
_dbSet = context.Set<T>();
}
public async Task<T?> GetByIdAsync(int id)
{
return await _dbSet.FindAsync(id);
}
public async Task<IEnumerable<T>> GetAllAsync()
{
return await _dbSet.ToListAsync();
}
public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
{
return await _dbSet.Where(predicate).ToListAsync();
}
public async Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate)
{
return await _dbSet.FirstOrDefaultAsync(predicate);
}
public async Task<bool> AnyAsync(Expression<Func<T, bool>> predicate)
{
return await _dbSet.AnyAsync(predicate);
}
public async Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null)
{
return predicate == null
? await _dbSet.CountAsync()
: await _dbSet.CountAsync(predicate);
}
public async Task<T> AddAsync(T entity)
{
await _dbSet.AddAsync(entity);
return entity;
}
public async Task<IEnumerable<T>> AddRangeAsync(IEnumerable<T> entities)
{
await _dbSet.AddRangeAsync(entities);
return entities;
}
public async Task UpdateAsync(T entity)
{
_dbSet.Update(entity);
await Task.CompletedTask;
}
public async Task UpdateRangeAsync(IEnumerable<T> entities)
{
_dbSet.UpdateRange(entities);
await Task.CompletedTask;
}
public async Task DeleteAsync(T entity)
{
_dbSet.Remove(entity);
await Task.CompletedTask;
}
public async Task DeleteRangeAsync(IEnumerable<T> entities)
{
_dbSet.RemoveRange(entities);
await Task.CompletedTask;
}
public async Task<bool> ExistsAsync(int id)
{
var entity = await _dbSet.FindAsync(id);
return entity != null;
}
public async Task SaveChangesAsync()
{
await _context.SaveChangesAsync();
}
}
}
