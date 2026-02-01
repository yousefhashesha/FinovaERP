using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Finova.Application.Contracts.Auth
{
    public interface IPermissionService
    {
        Task<IReadOnlySet<string>> GetUserPermissionCodesAsync(Guid userId, CancellationToken ct = default);
    }
}
