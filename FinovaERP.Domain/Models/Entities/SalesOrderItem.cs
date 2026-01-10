using System;
using System.ComponentModel.DataAnnotations;
using FinovaERP.Domain.Interfaces;

namespace FinovaERP.Domain.Models.Entities
{
    /// <summary>
    /// Sales order item with complete details
    /// </summary>
    public class SalesOrderItem : BaseEntity
    {
        public int SalesOrderId { get; set; }
        public virtual SalesOrder SalesOrder { get; set; } = null!;

        public int ItemId { get; set; }
        public virtual Item Item { get; set; } = null!;

        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }

        [Range(0, 999999999999.99)]
        public decimal UnitPrice { get; set; }

        [Range(0, 999999999999.99)]
        public decimal TotalPrice { get; set; }

        [Range(0, 100)]
        public decimal DiscountPercent { get; set; }

        [Range(0, 999999999999.99)]
        public decimal DiscountAmount { get; set; }

        [Range(0, 100)]
        public decimal TaxPercent { get; set; }

        [Range(0, 999999999999.99)]
        public decimal TaxAmount { get; set; }

        [Range(0, 999999999999.99)]
        public decimal NetAmount { get; set; }

        // Calculated properties
        public decimal GrossAmount => Quantity * UnitPrice;
        public decimal TotalDiscount => DiscountAmount + (GrossAmount * DiscountPercent / 100);
        public decimal TotalTax => TaxAmount + ((GrossAmount - TotalDiscount) * TaxPercent / 100);
    }
}
