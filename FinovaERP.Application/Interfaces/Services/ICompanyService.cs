using FinovaERP.Domain.Models.Entities;

namespace FinovaERP.Application.Interfaces.Services
{
    /// <summary>
    /// Company service interface for company management
    /// </summary>
    public interface ICompanyService
    {
        Task<Company?> GetCompanyByIdAsync(int id);
        Task<IEnumerable<Company>> GetAllCompaniesAsync();
        Task<Company> CreateCompanyAsync(Company company);
        Task UpdateCompanyAsync(Company company);
        Task DeleteCompanyAsync(int id);
        Task<IEnumerable<Company>> GetUserCompaniesAsync(int userId);
    }
}
