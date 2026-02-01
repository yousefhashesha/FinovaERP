using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Finova.Application.Contracts.Auth
{
    public sealed record CompanyDto(Guid CompanyId, string CompanyCode, string CompanyName, bool IsActive);

    public interface ICompanyService
    {
        Task<IReadOnlyList<CompanyDto>> GetUserCompaniesAsync(Guid userId, CancellationToken ct = default);
        Task<CompanyDto?> GetDefaultCompanyAsync(Guid userId, CancellationToken ct = default);
    }
}
