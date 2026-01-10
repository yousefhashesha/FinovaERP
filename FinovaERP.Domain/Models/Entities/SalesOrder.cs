using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using FinovaERP.Domain.Interfaces;

namespace FinovaERP.Domain.Models.Entities
{
    /// <summary>
    /// Sales order with complete tracking
    /// </summary>
    public class SalesOrder : BaseEntity
    {
        [Required]
        [StringLength(50)]
        public string OrderNumber { get; set; } = string.Empty;

        public DateTime OrderDate { get; set; } = DateTime.UtcNow;

        public int CustomerId { get; set; }
        public virtual Customer Customer { get; set; } = null!;

        public int CurrencyId { get; set; }
        public virtual Currency Currency { get; set; } = null!;

        [Range(0, 999999999999.99)]
        public decimal SubTotal { get; set; }

        [Range(0, 999999999999.99)]
        public decimal TaxAmount { get; set; }

        [Range(0, 999999999999.99)]
        public decimal TotalAmount { get; set; }

        [StringLength(20)]
        public string Status { get; set; } = "Draft";

        [StringLength(1000)]
        public string? Notes { get; set; }

        public virtual ICollection<SalesOrderItem> Items { get; set; } = new List<SalesOrderItem>();

        // Calculated properties
        public decimal TaxPercent => SubTotal > 0 ? (TaxAmount / SubTotal) * 100 : 0;
        public bool IsEditable => Status == "Draft" || Status == "Pending";
        public bool IsCompleted => Status == "Completed" || Status == "Paid";
    }
}
