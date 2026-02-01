using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Finova.Application.Contracts.Auth;
using Finova.Infrastructure.Repositories;

namespace Finova.Infrastructure.Services
{
    public sealed class PermissionService : IPermissionService
    {
        private readonly PermissionRepository _repo;

        public PermissionService(PermissionRepository repo) => _repo = repo;

        public Task<IReadOnlySet<string>> GetUserPermissionCodesAsync(Guid userId, CancellationToken ct = default)
            => _repo.GetUserPermissionCodesAsync(userId, ct);
    }
}
