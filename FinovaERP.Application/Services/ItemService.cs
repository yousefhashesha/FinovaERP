using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using FinovaERP.Application.Interfaces;
using FinovaERP.Application.Interfaces.Services;
using FinovaERP.Domain.Models.Entities;
using FinovaERP.Infrastructure.Data;
namespace FinovaERP.Application.Services
{
/// <summary>
/// Advanced item management service implementation
/// </summary>
public class ItemService : IItemService
{
private readonly IRepository<Item> _itemRepository;
private readonly IRepository<ItemCategory> _categoryRepository;
private readonly IRepository<NumberSequence> _numberSequenceRepository;
private readonly IRepository<InventoryTransaction> _inventoryTransactionRepository;
private readonly IUnitOfWork _unitOfWork;
private readonly FinovaDbContext _context;
public ItemService(
IRepository<Item> itemRepository,
IRepository<ItemCategory> categoryRepository,
IRepository<NumberSequence> numberSequenceRepository,
IRepository<InventoryTransaction> inventoryTransactionRepository,
IUnitOfWork unitOfWork,
FinovaDbContext context)
{
_itemRepository = itemRepository;
_categoryRepository = categoryRepository;
_numberSequenceRepository = numberSequenceRepository;
_inventoryTransactionRepository = inventoryTransactionRepository;
_unitOfWork = unitOfWork;
_context = context;
}
public async Task<Item?> GetItemByIdAsync(int id)
{
return await _itemRepository.GetByIdAsync(id);
}
public async Task<IEnumerable<Item>> GetAllItemsAsync()
{
return await _context.Items
.Include(i => i.Category)
.Where(i => i.IsActive)
.OrderBy(i => i.Name)
.ToListAsync();
}
public async Task<IEnumerable<Item>> GetItemsByCategoryAsync(int categoryId)
{
return await _context.Items
.Include(i => i.Category)
.Where(i => i.CategoryId == categoryId && i.IsActive)
.OrderBy(i => i.Name)
.ToListAsync();
}
public async Task<IEnumerable<Item>> SearchItemsAsync(string searchTerm, int? categoryId = null)
{
var query = _context.Items
.Include(i => i.Category)
.Where(i => i.IsActive);
if (!string.IsNullOrWhiteSpace(searchTerm))
{
query = query.Where(i =>
i.Name.Contains(searchTerm) ||
i.Code.Contains(searchTerm) ||
(i.Description != null && i.Description.Contains(searchTerm)));
}
if (categoryId.HasValue)
{
query = query.Where(i => i.CategoryId == categoryId.Value);
}
return await query.OrderBy(i => i.Name).ToListAsync();
}
public async Task<Item> CreateItemAsync(Item item)
{
// Generate item code if not provided
if (string.IsNullOrWhiteSpace(item.Code))
{
item.Code = await GenerateItemCodeAsync();
}
// Validate category exists
var categoryExists = await _categoryRepository.ExistsAsync(item.CategoryId);
if (!categoryExists)
{
throw new ArgumentException("Invalid category ID");
}
// Check if code already exists
var codeExists = await ItemExistsAsync(item.Code);
if (codeExists)
{
throw new ArgumentException("Item code already exists");
}
item.CreatedDate = DateTime.UtcNow;
item.IsActive = true;
await _unitOfWork.BeginTransactionAsync();
try
{
var createdItem = await _itemRepository.AddAsync(item);
await _unitOfWork.SaveChangesAsync();
// Create initial inventory transaction
if (item.CurrentStock > 0)
{
var transaction = new InventoryTransaction
{
ItemId = createdItem.Id,
TransactionType = "Initial Stock",
Quantity = item.CurrentStock,
UnitCost = item.CostPrice,
TotalCost = item.CurrentStock * item.CostPrice,
TransactionDate = DateTime.UtcNow,
CreatedBy = item.CreatedBy
};
await _inventoryTransactionRepository.AddAsync(transaction);
await _unitOfWork.SaveChangesAsync();
}
await _unitOfWork.CommitTransactionAsync();
return createdItem;
}
catch
{
await _unitOfWork.RollbackTransactionAsync();
throw;
}
}
public async Task<Item> UpdateItemAsync(Item item)
{
var existingItem = await _itemRepository.GetByIdAsync(item.Id);
if (existingItem == null)
{
throw new KeyNotFoundException("Item not found");
}
// Track stock changes
var stockDifference = item.CurrentStock - existingItem.CurrentStock;
existingItem.Name = item.Name;
existingItem.Description = item.Description;
existingItem.CategoryId = item.CategoryId;
existingItem.Unit = item.Unit;
existingItem.CostPrice = item.CostPrice;
existingItem.SellingPrice = item.SellingPrice;
existingItem.MinimumStock = item.MinimumStock;
existingItem.CurrentStock = item.CurrentStock;
existingItem.Barcode = item.Barcode;
existingItem.ImagePath = item.ImagePath;
existingItem.ModifiedDate = DateTime.UtcNow;
existingItem.ModifiedBy = item.ModifiedBy;
await _unitOfWork.BeginTransactionAsync();
try
{
await _itemRepository.UpdateAsync(existingItem);
await _unitOfWork.SaveChangesAsync();
// Create inventory transaction for stock changes
if (stockDifference != 0)
{
var transaction = new InventoryTransaction
{
ItemId = item.Id,
TransactionType = stockDifference > 0 ? "Stock Adjustment (+)" : "Stock Adjustment (-)",
Quantity = Math.Abs(stockDifference),
UnitCost = item.CostPrice,
TotalCost = Math.Abs(stockDifference) * item.CostPrice,
TransactionDate = DateTime.UtcNow,
CreatedBy = item.ModifiedBy ?? "System"
};
await _inventoryTransactionRepository.AddAsync(transaction);
await _unitOfWork.SaveChangesAsync();
}
await _unitOfWork.CommitTransactionAsync();
return existingItem;
}
catch
{
await _unitOfWork.RollbackTransactionAsync();
throw;
}
}
public async Task<bool> DeleteItemAsync(int id)
{
var item = await _itemRepository.GetByIdAsync(id);
if (item == null)
{
return false;
}
item.IsActive = false;
item.ModifiedDate = DateTime.UtcNow;
await _itemRepository.UpdateAsync(item);
await _unitOfWork.SaveChangesAsync();
return true;
}
public async Task<bool> UpdateStockAsync(int itemId, int quantity, string transactionType)
{
var item = await _itemRepository.GetByIdAsync(itemId);
if (item == null)
{
return false;
}
await _unitOfWork.BeginTransactionAsync();
try
{
// Update item stock
if (transactionType == "Stock In")
{
item.CurrentStock += quantity;
}
else if (transactionType == "Stock Out")
{
if (item.CurrentStock < quantity)
{
throw new InvalidOperationException("Insufficient stock");
}
item.CurrentStock -= quantity;
}
item.ModifiedDate = DateTime.UtcNow;
await _itemRepository.UpdateAsync(item);
await _unitOfWork.SaveChangesAsync();
// Create inventory transaction
var transaction = new InventoryTransaction
{
ItemId = itemId,
TransactionType = transactionType,
Quantity = quantity,
UnitCost = item.CostPrice,
TotalCost = quantity * item.CostPrice,
TransactionDate = DateTime.UtcNow,
CreatedBy = "System"
};
await _inventoryTransactionRepository.AddAsync(transaction);
await _unitOfWork.SaveChangesAsync();
await _unitOfWork.CommitTransactionAsync();
return true;
}
catch
{
await _unitOfWork.RollbackTransactionAsync();
throw;
}
}
public async Task<IEnumerable<Item>> GetLowStockItemsAsync()
{
return await _context.Items
.Include(i => i.Category)
.Where(i => i.IsActive && i.CurrentStock <= i.MinimumStock)
.OrderBy(i => i.Name)
.ToListAsync();
}
public async Task<decimal> GetItemTotalValueAsync(int itemId)
{
var item = await _itemRepository.GetByIdAsync(itemId);
return item?.TotalValue ?? 0;
}
public async Task<string> GenerateItemCodeAsync()
{
var sequence = await _numberSequenceRepository
.FirstOrDefaultAsync(ns => ns.Code == "ITEM" && ns.IsActive);
if (sequence == null)
{
return "ITM" + DateTime.UtcNow.ToString("yyyyMMdd") + "001";
}
sequence.LastNumber++;
await _numberSequenceRepository.UpdateAsync(sequence);
await _unitOfWork.SaveChangesAsync();
string code = sequence.Prefix + sequence.LastNumber.ToString().PadLeft(sequence.MinDigits, '0');
return code;
}
public async Task<bool> ItemExistsAsync(string code)
{
return await _itemRepository.AnyAsync(i => i.Code == code && i.IsActive);
}
public async Task<int> GetTotalItemCountAsync()
{
return await _itemRepository.CountAsync(i => i.IsActive);
}
public async Task<Dictionary<string, int>> GetItemsByCategoryAsync()
{
return await _context.Items
.Where(i => i.IsActive)
.GroupBy(i => i.Category.Name)
.ToDictionaryAsync(g => g.Key, g => g.Count());
}
}
}
