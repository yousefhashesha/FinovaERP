using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FinovaERP.Domain.Models.Entities;
namespace FinovaERP.Application.Interfaces.Services
{
/// <summary>
/// Advanced item management service
/// </summary>
public interface IItemService
{
Task<Item?> GetItemByIdAsync(int id);
Task<IEnumerable<Item>> GetAllItemsAsync();
Task<IEnumerable<Item>> GetItemsByCategoryAsync(int categoryId);
Task<IEnumerable<Item>> SearchItemsAsync(string searchTerm, int? categoryId = null);
Task<Item> CreateItemAsync(Item item);
Task<Item> UpdateItemAsync(Item item);
Task<bool> DeleteItemAsync(int id);
Task<bool> UpdateStockAsync(int itemId, int quantity, string transactionType);
Task<IEnumerable<Item>> GetLowStockItemsAsync();
Task<decimal> GetItemTotalValueAsync(int itemId);
Task<string> GenerateItemCodeAsync();
Task<bool> ItemExistsAsync(string code);
Task<int> GetTotalItemCountAsync();
Task<Dictionary<string, int>> GetItemsByCategoryAsync();
}
}
