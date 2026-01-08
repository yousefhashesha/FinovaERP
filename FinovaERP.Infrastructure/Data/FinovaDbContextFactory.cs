using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace FinovaERP.Infrastructure.Data
{
    /// <summary>
    /// Design-time factory for Entity Framework migrations
    /// </summary>
    public class FinovaDbContextFactory : IDesignTimeDbContextFactory<FinovaDbContext>
    {
        public FinovaDbContext CreateDbContext(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile(""appsettings.json"", optional: false)
                .Build();

            var optionsBuilder = new DbContextOptionsBuilder<FinovaDbContext>();
            var connectionString = configuration.GetConnectionString(""DefaultConnection"");
            
            optionsBuilder.UseSqlServer(connectionString);

            return new FinovaDbContext(optionsBuilder.Options);
        }
    }
}
