using System.Collections.Generic;
using FinovaERP.Domain.Interfaces;
namespace FinovaERP.Domain.Models.Entities
{
/// <summary>
/// Item category for organizing products
/// </summary>
public class ItemCategory : BaseEntity
{
public string Name { get; set; } = string.Empty;
public string? Description { get; set; }
public int? ParentId { get; set; }
public virtual ItemCategory? Parent { get; set; }
public virtual ICollection<ItemCategory> Children { get; set; } = new List<ItemCategory>();
public virtual ICollection<Item> Items { get; set; } = new List<Item>();
}
}
