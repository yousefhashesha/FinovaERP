using System.ComponentModel.DataAnnotations;
using FinovaERP.Domain.Interfaces;

namespace FinovaERP.Domain.Models.Entities
{
    /// <summary>
    /// Supplier information with full details
    /// </summary>
    public class Supplier : BaseEntity
    {
        [Required]
        [StringLength(50)]
        public string Code { get; set; } = string.Empty;

        [Required]
        [StringLength(200)]
        public string Name { get; set; } = string.Empty;

        [StringLength(200)]
        public string? ContactPerson { get; set; }

        [StringLength(200)]
        [EmailAddress]
        public string? Email { get; set; }

        [StringLength(50)]
        [Phone]
        public string? Phone { get; set; }

        [StringLength(50)]
        public string? Mobile { get; set; }

        [StringLength(500)]
        public string? Address { get; set; }

        [StringLength(100)]
        public string? City { get; set; }

        [StringLength(100)]
        public string? State { get; set; }

        [StringLength(100)]
        public string? Country { get; set; }

        [StringLength(20)]
        public string? PostalCode { get; set; }

        [StringLength(50)]
        public string? TaxNumber { get; set; }

        [Range(0, 365)]
        public int PaymentTerms { get; set; } = 30;

        [StringLength(500)]
        public string? BankAccount { get; set; }

        [StringLength(100)]
        public string? BankName { get; set; }

        public string FullAddress => $"{Address}, {City}, {State}, {Country} {PostalCode}";
        public bool IsLocal => Country?.ToLower() == "usa" || Country?.ToLower() == "united states";
    }
}
