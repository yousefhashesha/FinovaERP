using System;
using System.Threading.Tasks;
namespace FinovaERP.Application.Interfaces
{
/// <summary>
/// Unit of Work pattern for transaction management
/// </summary>
public interface IUnitOfWork : IDisposable
{
IRepository<T> GetRepository<T>() where T : class;
Task<int> SaveChangesAsync();
Task BeginTransactionAsync();
Task CommitTransactionAsync();
Task RollbackTransactionAsync();
}
}
