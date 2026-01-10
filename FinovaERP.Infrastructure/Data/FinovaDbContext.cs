using System;
using Microsoft.EntityFrameworkCore;
using FinovaERP.Domain.Models.Entities;
using Microsoft.Extensions.Configuration;

namespace FinovaERP.Infrastructure.Data
{
    /// <summary>
    /// Database context for FinovaERP with Entity Framework Core
    /// </summary>
    public class FinovaDbContext : DbContext
    {
        public FinovaDbContext(DbContextOptions<FinovaDbContext> options) : base(options)
        {
        }

        // Entities
        public DbSet<Company> Companies { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<Item> Items { get; set; }
        public DbSet<ItemCategory> ItemCategories { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Supplier> Suppliers { get; set; }
        public DbSet<SalesOrder> SalesOrders { get; set; }
        public DbSet<SalesOrderItem> SalesOrderItems { get; set; }
        public DbSet<PurchaseOrder> PurchaseOrders { get; set; }
        public DbSet<PurchaseOrderItem> PurchaseOrderItems { get; set; }
        public DbSet<InventoryTransaction> InventoryTransactions { get; set; }
        public DbSet<Currency> Currencies { get; set; }
        public DbSet<NumberSequence> NumberSequences { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure relationships
            modelBuilder.Entity<UserRole>()
                .HasKey(ur => new { ur.UserId, ur.RoleId });

            modelBuilder.Entity<UserRole>()
                .HasOne(ur => ur.User)
                .WithMany(u => u.UserRoles)
                .HasForeignKey(ur => ur.UserId);

            modelBuilder.Entity<UserRole>()
                .HasOne(ur => ur.Role)
                .WithMany(r => r.UserRoles)
                .HasForeignKey(ur => ur.RoleId);

            // Configure Item relationships
            modelBuilder.Entity<Item>()
                .HasOne(i => i.Category)
                .WithMany(c => c.Items)
                .HasForeignKey(i => i.CategoryId);

            // Configure Sales Order relationships
            modelBuilder.Entity<SalesOrderItem>()
                .HasOne(soi => soi.SalesOrder)
                .WithMany(so => so.Items)
                .HasForeignKey(soi => soi.SalesOrderId);

            modelBuilder.Entity<SalesOrderItem>()
                .HasOne(soi => soi.Item)
                .WithMany()
                .HasForeignKey(soi => soi.ItemId);

            // Configure Purchase Order relationships
            modelBuilder.Entity<PurchaseOrderItem>()
                .HasOne(poi => poi.PurchaseOrder)
                .WithMany(po => po.Items)
                .HasForeignKey(poi => poi.PurchaseOrderId);

            modelBuilder.Entity<PurchaseOrderItem>()
                .HasOne(poi => poi.Item)
                .WithMany()
                .HasForeignKey(poi => poi.ItemId);

            // Seed initial data
            SeedInitialData(modelBuilder);
        }

        private void SeedInitialData(ModelBuilder modelBuilder)
        {
            // Seed Companies
            modelBuilder.Entity<Company>().HasData(
                new Company { Id = 1, Name = "Main Company", Code = "MAIN", Address = "Main Street 123", Phone = "+1234567890", Email = "info@maincompany.com", CreatedDate = DateTime.UtcNow, CreatedBy = "System", IsActive = true }
            );

            // Seed Users
            modelBuilder.Entity<User>().HasData(
                new User { Id = 1, Username = "admin", Password = "admin123", Email = "admin@finovaerp.com", FirstName = "System", LastName = "Administrator", CompanyId = 1, CreatedDate = DateTime.UtcNow, CreatedBy = "System", IsActive = true }
            );

            // Seed Roles
            modelBuilder.Entity<Role>().HasData(
                new Role { Id = 1, Name = "Administrator", Description = "System Administrator", CreatedDate = DateTime.UtcNow, CreatedBy = "System", IsActive = true },
                new Role { Id = 2, Name = "Manager", Description = "Department Manager", CreatedDate = DateTime.UtcNow, CreatedBy = "System", IsActive = true },
                new Role { Id = 3, Name = "User", Description = "Regular User", CreatedDate = DateTime.UtcNow, CreatedBy = "System", IsActive = true }
            );

            // Seed UserRoles
            modelBuilder.Entity<UserRole>().HasData(
                new UserRole { UserId = 1, RoleId = 1 }
            );

            // Seed Currencies
            modelBuilder.Entity<Currency>().HasData(
                new Currency { Id = 1, Code = "USD", Name = "US Dollar", Symbol = "$", ExchangeRate = 1.0m, IsActive = true, CreatedDate = DateTime.UtcNow, CreatedBy = "System" },
                new Currency { Id = 2, Code = "EUR", Name = "Euro", Symbol = "€", ExchangeRate = 0.85m, IsActive = true, CreatedDate = DateTime.UtcNow, CreatedBy = "System" }
            );

            // Seed Item Categories
            modelBuilder.Entity<ItemCategory>().HasData(
                new ItemCategory { Id = 1, Name = "Electronics", Description = "Electronic devices and accessories", CreatedDate = DateTime.UtcNow, CreatedBy = "System", IsActive = true },
                new ItemCategory { Id = 2, Name = "Clothing", Description = "Clothing and apparel", CreatedDate = DateTime.UtcNow, CreatedBy = "System", IsActive = true },
                new ItemCategory { Id = 3, Name = "Food", Description = "Food and beverages", CreatedDate = DateTime.UtcNow, CreatedBy = "System", IsActive = true }
            );

            // Seed Number Sequences
            modelBuilder.Entity<NumberSequence>().HasData(
                new NumberSequence { Id = 1, Code = "ITEM", Description = "Item Numbers", Prefix = "ITM", LastNumber = 1000, MinDigits = 4, CreatedDate = DateTime.UtcNow, CreatedBy = "System", IsActive = true },
                new NumberSequence { Id = 2, Code = "CUST", Description = "Customer Numbers", Prefix = "CUST", LastNumber = 1000, MinDigits = 4, CreatedDate = DateTime.UtcNow, CreatedBy = "System", IsActive = true },
                new NumberSequence { Id = 3, Code = "SUPP", Description = "Supplier Numbers", Prefix = "SUPP", LastNumber = 1000, MinDigits = 4, CreatedDate = DateTime.UtcNow, CreatedBy = "System", IsActive = true }
            );
        }
    }
}
