using System;
using System.ComponentModel.DataAnnotations;
using FinovaERP.Domain.Interfaces;

namespace FinovaERP.Domain.Models.Entities
{
    /// <summary>
    /// Purchase order item details
    /// </summary>
    public class PurchaseOrderItem : BaseEntity
    {
        public int PurchaseOrderId { get; set; }
        public virtual PurchaseOrder PurchaseOrder { get; set; } = null!;

        public int ItemId { get; set; }
        public virtual Item Item { get; set; } = null!;

        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }

        [Range(0, 999999999999.99)]
        public decimal UnitPrice { get; set; }

        [Range(0, 999999999999.99)]
        public decimal TotalPrice { get; set; }

        [StringLength(500)]
        public string? Notes { get; set; }

        public DateTime? ExpectedDeliveryDate { get; set; }

        // Calculated properties
        public decimal GrossAmount => Quantity * UnitPrice;
    }
}
