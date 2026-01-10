using System.ComponentModel.DataAnnotations;
using FinovaERP.Domain.Interfaces;

namespace FinovaERP.Domain.Models.Entities
{
    /// <summary>
    /// Currency information for multi-currency support
    /// </summary>
    public class Currency : BaseEntity
    {
        [Required]
        [StringLength(3)]
        public string Code { get; set; } = string.Empty; // USD, EUR, etc.

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [StringLength(10)]
        public string? Symbol { get; set; } // $, €, etc.

        [Range(0.000001, 999999.999999)]
        public decimal ExchangeRate { get; set; } = 1;

        public bool IsBaseCurrency { get; set; } = false;

        public string FormatAmount(decimal amount)
        {
            if (!string.IsNullOrEmpty(Symbol))
                return $""{Symbol}{amount:N2}"";
            else
                return $""{Code} {amount:N2}"";
        }

        public decimal ConvertFromBaseCurrency(decimal baseAmount)
        {
            if (IsBaseCurrency) return baseAmount;
            return baseAmount * ExchangeRate;
        }

        public decimal ConvertToBaseCurrency(decimal amount)
        {
            if (IsBaseCurrency) return amount;
            return amount / ExchangeRate;
        }
    }
}
