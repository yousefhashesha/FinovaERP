using System;
using System.ComponentModel.DataAnnotations;
using FinovaERP.Domain.Interfaces;
namespace FinovaERP.Domain.Models.Entities
{
/// <summary>
/// Product/Service item with complete inventory tracking
/// </summary>
public class Item : BaseEntity
{
[Required]
[StringLength(50)]
public string Code { get; set; } = string.Empty;
[Required]
[StringLength(200)]
public string Name { get; set; } = string.Empty;
[StringLength(1000)]
public string? Description { get; set; }
public int CategoryId { get; set; }
public virtual ItemCategory Category { get; set; } = null!;
[StringLength(20)]
public string? Unit { get; set; }
[Range(0, 999999999999.99)]
public decimal CostPrice { get; set; }
[Range(0, 999999999999.99)]
public decimal SellingPrice { get; set; }
[Range(0, int.MaxValue)]
public int MinimumStock { get; set; }
[Range(0, int.MaxValue)]
public int CurrentStock { get; set; }
[StringLength(50)]
public string? Barcode { get; set; }
[StringLength(500)]
public string? ImagePath { get; set; }
// Calculated properties
public decimal ProfitMargin => SellingPrice > 0 ? ((SellingPrice - CostPrice) / SellingPrice) * 100 : 0;
public bool IsLowStock => CurrentStock <= MinimumStock;
public decimal TotalValue => CurrentStock * CostPrice;
}
}
