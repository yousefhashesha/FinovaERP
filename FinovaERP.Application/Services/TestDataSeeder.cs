using FinovaERP.Application.Interfaces.Services;
using FinovaERP.Domain.Models.Entities;
using FinovaERP.Infrastructure.Repositories;

namespace FinovaERP.Application.Services
{
    /// <summary>
    /// Test data seeder for initial system setup
    /// </summary>
    public class TestDataSeeder
    {
        private readonly IRepository<User> _userRepository;
        private readonly IRepository<Company> _companyRepository;
        private readonly IRepository<Role> _roleRepository;
        private readonly IRepository<UserRole> _userRoleRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IUnitOfWork _unitOfWork;

        public TestDataSeeder(
            IRepository<User> userRepository,
            IRepository<Company> companyRepository,
            IRepository<Role> roleRepository,
            IRepository<UserRole> userRoleRepository,
            IPasswordHasher passwordHasher,
            IUnitOfWork unitOfWork)
        {
            _userRepository = userRepository;
            _companyRepository = companyRepository;
            _roleRepository = roleRepository;
            _userRoleRepository = userRoleRepository;
            _passwordHasher = passwordHasher;
            _unitOfWork = unitOfWork;
        }

        public async Task SeedDefaultDataAsync()
        {
            // Check if data already exists
            var existingUsers = await _userRepository.GetAllAsync();
            if (existingUsers.Any())
            {
                return; // Data already seeded
            }

            // Create default admin user
            var adminUser = new User
            {
                Username = ""admin"",
                Email = ""admin@finovaerp.com"",
                PasswordHash = _passwordHasher.HashPassword(""Admin123!""),
                FirstName = ""System"",
                LastName = ""Administrator"",
                PhoneNumber = ""+1-555-0100"",
                IsActive = true,
                CreatedBy = ""System""
            };

            await _userRepository.AddAsync(adminUser);
            await _unitOfWork.SaveChangesAsync();

            // Get default company and roles
            var companies = await _companyRepository.GetAllAsync();
            var roles = await _roleRepository.GetAllAsync();
            
            var defaultCompany = companies.FirstOrDefault();
            var adminRole = roles.FirstOrDefault(r => r.Name == ""System Administrator"");

            if (defaultCompany != null && adminRole != null)
            {
                // Assign admin role to admin user
                var userRole = new UserRole
                {
                    UserId = adminUser.Id,
                    RoleId = adminRole.Id,
                    CompanyId = defaultCompany.Id,
                    AssignedBy = ""System"",
                    IsActive = true,
                    CreatedBy = ""System""
                };

                await _userRoleRepository.AddAsync(userRole);
                await _unitOfWork.SaveChangesAsync();
            }

            // Create test company
            var testCompany = new Company
            {
                Name = ""Tech Solutions Inc."",
                TradeName = ""TechSol"",
                TaxNumber = ""123456789"",
                Phone = ""+1-555-0200"",
                Email = ""info@techsol.com"",
                Website = ""www.techsol.com"",
                Address = ""123 Business St"",
                City = ""Tech City"",
                State = ""TC"",
                Country = ""USA"",
                PostalCode = ""12345"",
                BaseCurrency = ""USD"",
                TimeZone = ""Eastern Standard Time"",
                FiscalYearStart = new DateTime(DateTime.Now.Year, 1, 1),
                IsActive = true,
                CreatedBy = adminUser.Username
            };

            await _companyRepository.AddAsync(testCompany);
            await _unitOfWork.SaveChangesAsync();

            // Create test users
            var testUsers = new[]
            {
                new User
                {
                    Username = ""manager1"",
                    Email = ""manager@techsol.com"",
                    PasswordHash = _passwordHasher.HashPassword(""Manager123!""),
                    FirstName = ""John"",
                    LastName = ""Manager"",
                    PhoneNumber = ""+1-555-0300"",
                    IsActive = true,
                    CreatedBy = adminUser.Username
                },
                new User
                {
                    Username = ""accountant1"",
                    Email = ""accountant@techsol.com"",
                    PasswordHash = _passwordHasher.HashPassword(""Accountant123!""),
                    FirstName = ""Sarah"",
                    LastName = ""Accountant"",
                    PhoneNumber = ""+1-555-0400"",
                    IsActive = true,
                    CreatedBy = adminUser.Username
                },
                new User
                {
                    Username = ""user1"",
                    Email = ""user@techsol.com"",
                    PasswordHash = _passwordHasher.HashPassword(""User123!""),
                    FirstName = ""Mike"",
                    LastName = ""User"",
                    PhoneNumber = ""+1-555-0500"",
                    IsActive = true,
                    CreatedBy = adminUser.Username
                }
            };

            foreach (var user in testUsers)
            {
                await _userRepository.AddAsync(user);
            }
            await _unitOfWork.SaveChangesAsync();

            // Assign roles to test users
            var companyAdminRole = roles.FirstOrDefault(r => r.Name == ""Company Admin"");
            var standardUserRole = roles.FirstOrDefault(r => r.Name == ""User"");

            if (companyAdminRole != null && standardUserRole != null)
            {
                // Manager gets Company Admin role
                await _userRoleRepository.AddAsync(new UserRole
                {
                    UserId = testUsers[0].Id,
                    RoleId = companyAdminRole.Id,
                    CompanyId = testCompany.Id,
                    AssignedBy = adminUser.Username,
                    IsActive = true,
                    CreatedBy = adminUser.Username
                });

                // Accountant gets Company Admin role
                await _userRoleRepository.AddAsync(new UserRole
                {
                    UserId = testUsers[1].Id,
                    RoleId = companyAdminRole.Id,
                    CompanyId = testCompany.Id,
                    AssignedBy = adminUser.Username,
                    IsActive = true,
                    CreatedBy = adminUser.Username
                });

                // Regular user gets User role
                await _userRoleRepository.AddAsync(new UserRole
                {
                    UserId = testUsers[2].Id,
                    RoleId = standardUserRole.Id,
                    CompanyId = testCompany.Id,
                    AssignedBy = adminUser.Username,
                    IsActive = true,
                    CreatedBy = adminUser.Username
                });

                await _unitOfWork.SaveChangesAsync();
            }
        }

        public async Task<bool> ValidateDefaultCredentialsAsync(string username, string password)
        {
            try
            {
                var users = await _userRepository.FindAsync(u => u.Username == username && u.IsActive);
                var user = users.FirstOrDefault();
                
                if (user == null)
                    return false;

                return _passwordHasher.VerifyPassword(password, user.PasswordHash);
            }
            catch
            {
                return false;
            }
        }
    }
}
