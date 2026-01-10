using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
namespace FinovaERP.Application.Interfaces
{
/// <summary>
/// Generic repository interface for CRUD operations
/// </summary>
/// <typeparam name="T">Entity type</typeparam>
public interface IRepository<T> where T : class
{
Task<T?> GetByIdAsync(int id);
Task<IEnumerable<T>> GetAllAsync();
Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);
Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate);
Task<bool> AnyAsync(Expression<Func<T, bool>> predicate);
Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null);
Task<T> AddAsync(T entity);
Task<IEnumerable<T>> AddRangeAsync(IEnumerable<T> entities);
Task UpdateAsync(T entity);
Task UpdateRangeAsync(IEnumerable<T> entities);
Task DeleteAsync(T entity);
Task DeleteRangeAsync(IEnumerable<T> entities);
Task<bool> ExistsAsync(int id);
Task SaveChangesAsync();
}
}
