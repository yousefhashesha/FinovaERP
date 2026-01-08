using System;
using System.Windows.Forms;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using FinovaERP.Application.Interfaces;
using FinovaERP.Application.Interfaces.Services;
using FinovaERP.Application.Services;
using FinovaERP.Infrastructure.Data;
using FinovaERP.Infrastructure.Repositories;
using FinovaERP.Presentation.Forms;
using Microsoft.Extensions.Configuration;

namespace FinovaERP.Presentation
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Build host with dependency injection
            var host = CreateHostBuilder().Build();
            
            // Initialize database
            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    var context = services.GetRequiredService<FinovaDbContext>();
                    context.Database.Migrate(); // Apply migrations
                    
                    // Seed default data
                    SeedDefaultData(services).Wait();
                }
                catch (Exception ex)
                {
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, ""An error occurred while migrating or initializing the database."");
                }
            }
            
            // Run application
            using (var serviceScope = host.Services.CreateScope())
            {
                var services = serviceScope.ServiceProvider;
                var loginForm = services.GetRequiredService<LoginForm>();
                Application.Run(loginForm);
            }
        }

        static IHostBuilder CreateHostBuilder() =>
            Host.CreateDefaultBuilder()
                .ConfigureServices((context, services) =>
                {
                    // Configure Entity Framework
                    services.AddDbContext<FinovaDbContext>(options =>
                        options.UseSqlServer(context.Configuration.GetConnectionString(""DefaultConnection"")));

                    // Register repositories
                    services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
                    services.AddScoped<IUnitOfWork, UnitOfWork>();

                    // Register specific repositories
                    services.AddScoped<IRepository<User>, UserRepository>();
                    services.AddScoped<IRepository<Company>, CompanyRepository>();
                    services.AddScoped<IRepository<Role>, RoleRepository>();
                    services.AddScoped<IRepository<UserRole>, Repository<UserRole>>();
                    services.AddScoped<IRepository<UserCompany>, Repository<UserCompany>>();

                    // Register services
                    services.AddScoped<IPasswordHasher, PasswordHasher>();
                    services.AddScoped<IAuthenticationService, AuthenticationService>();
                    services.AddScoped<IUserService, UserService>();
                    services.AddScoped<ICompanyService, CompanyService>();
                    services.AddScoped<ISessionService, SessionService>(); // You'll need to implement this

                    // Register forms
                    services.AddTransient<LoginForm>();
                    services.AddTransient<MainForm>();
                    services.AddTransient<DashboardForm>();

                    // Register test data seeder
                    services.AddScoped<TestDataSeeder>();
                });

        static async Task SeedDefaultData(IServiceProvider services)
        {
            var seeder = services.GetRequiredService<TestDataSeeder>();
            await seeder.SeedDefaultDataAsync();
        }
    }
}
