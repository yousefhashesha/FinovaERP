using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Finova.Application.Contracts.Auth;
using Finova.Infrastructure.Repositories;

namespace Finova.Infrastructure.Services
{
    public sealed class CompanyService : ICompanyService
    {
        private readonly CompanyRepository _repo;

        public CompanyService(CompanyRepository repo) => _repo = repo;

        public Task<IReadOnlyList<CompanyDto>> GetUserCompaniesAsync(Guid userId, CancellationToken ct = default)
            => _repo.GetUserCompaniesAsync(userId, ct);

        public Task<CompanyDto?> GetDefaultCompanyAsync(Guid userId, CancellationToken ct = default)
            => _repo.GetDefaultCompanyAsync(userId, ct);
    }
}
