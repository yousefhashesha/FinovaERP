using FinovaERP.Application.Interfaces;
using FinovaERP.Application.Interfaces.Services;
using FinovaERP.Domain.Models.Entities;
using System.Linq.Expressions;

namespace FinovaERP.Application.Services
{
    /// <summary>
    /// Company service implementation for company management operations
    /// </summary>
    public class CompanyService : ICompanyService
    {
        private readonly IRepository<Company> _companyRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CompanyService(IRepository<Company> companyRepository, IUnitOfWork unitOfWork)
        {
            _companyRepository = companyRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Company?> GetCompanyByIdAsync(int id)
        {
            return await _companyRepository.GetByIdAsync(id);
        }

        public async Task<IEnumerable<Company>> GetAllCompaniesAsync()
        {
            return await _companyRepository.FindAsync(c => c.IsActive);
        }

        public async Task<Company> CreateCompanyAsync(Company company)
        {
            if (company == null)
                throw new ArgumentNullException(nameof(company));

            // Validate company name uniqueness
            if (!await IsNameUniqueAsync(company.Name))
                throw new InvalidOperationException(""Company name already exists"");

            // Validate tax number uniqueness if provided
            if (!string.IsNullOrWhiteSpace(company.TaxNumber) && !await IsTaxNumberUniqueAsync(company.TaxNumber))
                throw new InvalidOperationException(""Tax number already exists"");

            // Set creation metadata
            company.CreatedDate = DateTime.Now;
            company.IsActive = true;

            var createdCompany = await _companyRepository.AddAsync(company);
            await _unitOfWork.SaveChangesAsync();

            return createdCompany;
        }

        public async Task UpdateCompanyAsync(Company company)
        {
            if (company == null)
                throw new ArgumentNullException(nameof(company));

            // Validate company name uniqueness (excluding current company)
            var existingCompany = await _companyRepository.GetByIdAsync(company.Id);
            if (existingCompany != null && existingCompany.Name != company.Name)
            {
                if (!await IsNameUniqueAsync(company.Name, company.Id))
                    throw new InvalidOperationException(""Company name already exists"");
            }

            // Validate tax number uniqueness if provided and changed
            if (!string.IsNullOrWhiteSpace(company.TaxNumber) && existingCompany?.TaxNumber != company.TaxNumber)
            {
                if (!await IsTaxNumberUniqueAsync(company.TaxNumber, company.Id))
                    throw new InvalidOperationException(""Tax number already exists"");
            }

            // Update metadata
            company.ModifiedDate = DateTime.Now;

            await _companyRepository.UpdateAsync(company);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteCompanyAsync(int id)
        {
            var company = await _companyRepository.GetByIdAsync(id);
            if (company != null)
            {
                company.IsActive = false;
                company.ModifiedDate = DateTime.Now;
                await _companyRepository.UpdateAsync(company);
                await _unitOfWork.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Company>> GetUserCompaniesAsync(int userId)
        {
            // This would need to be implemented with a join through UserCompanies
            // For now, returning all active companies
            return await _companyRepository.FindAsync(c => c.IsActive);
        }

        private async Task<bool> IsNameUniqueAsync(string name, int? excludeCompanyId = null)
        {
            var existingCompanies = await _companyRepository.FindAsync(c => c.Name == name && c.IsActive);
            if (excludeCompanyId.HasValue)
            {
                existingCompanies = existingCompanies.Where(c => c.Id != excludeCompanyId.Value);
            }
            return !existingCompanies.Any();
        }

        private async Task<bool> IsTaxNumberUniqueAsync(string taxNumber, int? excludeCompanyId = null)
        {
            var existingCompanies = await _companyRepository.FindAsync(c => c.TaxNumber == taxNumber && c.IsActive);
            if (excludeCompanyId.HasValue)
            {
                existingCompanies = existingCompanies.Where(c => c.Id != excludeCompanyId.Value);
            }
            return !existingCompanies.Any();
        }
    }
}
