using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using FinovaERP.Application.Interfaces;
using FinovaERP.Infrastructure.Data;
namespace FinovaERP.Infrastructure.Repositories
{
/// <summary>
/// Unit of Work implementation for transaction management
/// </summary>
public class UnitOfWork : IUnitOfWork
{
private readonly FinovaDbContext _context;
private Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction? _currentTransaction;
public UnitOfWork(FinovaDbContext context)
{
_context = context;
}
public IRepository<T> GetRepository<T>() where T : class
{
return new Repository<T>(_context);
}
public async Task<int> SaveChangesAsync()
{
return await _context.SaveChangesAsync();
}
public async Task BeginTransactionAsync()
{
if (_currentTransaction != null)
{
throw new InvalidOperationException("Transaction already started");
}
_currentTransaction = await _context.Database.BeginTransactionAsync();
}
public async Task CommitTransactionAsync()
{
if (_currentTransaction == null)
{
throw new InvalidOperationException("No active transaction");
}
try
{
await _context.SaveChangesAsync();
await _currentTransaction.CommitAsync();
}
catch
{
await _currentTransaction.RollbackAsync();
throw;
}
finally
{
await _currentTransaction.DisposeAsync();
_currentTransaction = null;
}
}
public async Task RollbackTransactionAsync()
{
if (_currentTransaction == null)
{
throw new InvalidOperationException("No active transaction");
}
try
{
await _currentTransaction.RollbackAsync();
}
finally
{
await _currentTransaction.DisposeAsync();
_currentTransaction = null;
}
}
public void Dispose()
{
_currentTransaction?.Dispose();
_context.Dispose();
}
}
}
