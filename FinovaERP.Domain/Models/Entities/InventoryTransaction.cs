using System;
using System.ComponentModel.DataAnnotations;
using FinovaERP.Domain.Interfaces;

namespace FinovaERP.Domain.Models.Entities
{
    /// <summary>
    /// Inventory transaction for stock tracking
    /// </summary>
    public class InventoryTransaction : BaseEntity
    {
        public int ItemId { get; set; }
        public virtual Item Item { get; set; } = null!;

        [Required]
        [StringLength(50)]
        public string TransactionType { get; set; } = string.Empty; // Stock In, Stock Out, Adjustment, Sale, Purchase

        public int? ReferenceId { get; set; } // SalesOrderId, PurchaseOrderId, etc.

        [StringLength(50)]
        public string? ReferenceType { get; set; } // SalesOrder, PurchaseOrder, etc.

        public int Quantity { get; set; } // Positive for stock in, negative for stock out

        [Range(0, 999999999999.99)]
        public decimal UnitCost { get; set; }

        [Range(0, 999999999999.99)]
        public decimal TotalCost { get; set; }

        public DateTime TransactionDate { get; set; } = DateTime.UtcNow;

        [StringLength(500)]
        public string? Notes { get; set; }

        [StringLength(100)]
        public string CreatedBy { get; set; } = string.Empty;

        // Calculated properties
        public bool IsStockIn => Quantity > 0;
        public bool IsStockOut => Quantity < 0;
        public int AbsoluteQuantity => Math.Abs(Quantity);
    }
}
