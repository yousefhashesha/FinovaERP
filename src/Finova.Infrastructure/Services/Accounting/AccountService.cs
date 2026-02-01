using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Finova.Application.Contracts.Accounting;
using Finova.Infrastructure.Repositories.Accounting;

namespace Finova.Infrastructure.Services.Accounting
{
    public sealed class AccountService : IAccountService
    {
        private readonly AccountRepository _repo;
        public AccountService(AccountRepository repo) => _repo = repo;

        public Task<IReadOnlyList<AccountDto>> GetChartAsync(Guid companyId, bool includeInactive, CancellationToken ct = default)
            => _repo.GetChartAsync(companyId, includeInactive, ct);

        public Task<AccountDto?> GetByIdAsync(Guid companyId, Guid accountId, CancellationToken ct = default)
            => _repo.GetByIdAsync(companyId, accountId, ct);

        public Task<Guid> CreateAsync(Guid companyId, string code, string name, string type, Guid? parentId, bool isPosting, string normalBalance, byte level, Guid? userId, CancellationToken ct = default)
            => _repo.CreateAsync(companyId, code, name, type, parentId, isPosting, normalBalance, level, userId, ct);

        public Task UpdateAsync(Guid companyId, Guid accountId, string code, string name, string type, Guid? parentId, bool isPosting, string normalBalance, byte level, bool isActive, Guid? userId, CancellationToken ct = default)
            => _repo.UpdateAsync(companyId, accountId, code, name, type, parentId, isPosting, normalBalance, level, isActive, userId, ct);
    }
}
