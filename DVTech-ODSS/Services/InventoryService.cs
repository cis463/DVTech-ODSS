using DVTech_ODSS.Data;
using DVTech_ODSS.Models;
using Microsoft.EntityFrameworkCore;

namespace DVTech_ODSS.Services
{
    public class InventoryService
    {
        private readonly ApplicationDbContext _context;

        public InventoryService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<InventoryItem>> GetAllItemsAsync()
        {
            return await _context.InventoryItems
                .Where(i => !i.IsArchived)
                .OrderBy(i => i.ItemName)
                .ToListAsync();
        }

        public async Task<List<InventoryItem>> GetArchivedItemsAsync()
        {
            return await _context.InventoryItems
                .Where(i => i.IsArchived)
                .OrderByDescending(i => i.ArchivedDate)
                .ToListAsync();
        }

        public async Task<InventoryItem?> GetItemByIdAsync(int id)
        {
            return await _context.InventoryItems.FindAsync(id);
        }

        public async Task<bool> AddItemAsync(InventoryItem item)
        {
            try
            {
                item.DateAdded = DateTime.Now;
                item.IsArchived = false;

                _context.InventoryItems.Add(item);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> UpdateItemAsync(InventoryItem item)
        {
            try
            {
                _context.InventoryItems.Update(item);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> ArchiveItemAsync(int id)
        {
            try
            {
                var item = await GetItemByIdAsync(id);
                if (item != null)
                {
                    item.IsArchived = true;
                    item.ArchivedDate = DateTime.Now;
                    await _context.SaveChangesAsync();
                    return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        // NEW: Smart restore that merges with existing items
        public async Task<bool> RestoreItemAsync(int id)
        {
            try
            {
                var archivedItem = await GetItemByIdAsync(id);
                if (archivedItem == null || !archivedItem.IsArchived)
                    return false;

                // Check if a matching active item exists (same name, brand, category)
                var existingItem = await _context.InventoryItems
                    .FirstOrDefaultAsync(i =>
                        !i.IsArchived &&
                        i.ItemName == archivedItem.ItemName &&
                        i.Brand == archivedItem.Brand &&
                        i.Category == archivedItem.Category);

                if (existingItem != null)
                {
                    // MERGE: Add archived quantity to existing item
                    existingItem.Quantity += archivedItem.Quantity;

                    // Update weighted average cost price
                    var totalValue = (existingItem.Quantity - archivedItem.Quantity) * existingItem.UnitPrice +
                                   archivedItem.Quantity * archivedItem.UnitPrice;
                    existingItem.UnitPrice = totalValue / existingItem.Quantity;

                    // Recalculate selling price based on current markup
                    existingItem.CalculateSellingPrice();

                    // Update last restocked date
                    existingItem.LastRestocked = DateTime.Now;

                    // Delete the archived item (no longer needed)
                    _context.InventoryItems.Remove(archivedItem);

                    await _context.SaveChangesAsync();
                    return true;
                }
                else
                {
                    // No matching item found - simply restore the archived item
                    archivedItem.IsArchived = false;
                    archivedItem.ArchivedDate = null;
                    archivedItem.LastRestocked = DateTime.Now;
                    await _context.SaveChangesAsync();
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }

        public async Task<List<InventoryItem>> GetLowStockItemsAsync()
        {
            return await _context.InventoryItems
                .Where(i => !i.IsArchived && i.Quantity <= i.MinimumStockLevel)
                .OrderBy(i => i.Quantity)
                .ToListAsync();
        }

        public async Task<List<InventoryItem>> GetItemsByCategoryAsync(string category)
        {
            return await _context.InventoryItems
                .Where(i => !i.IsArchived && i.Category == category)
                .OrderBy(i => i.ItemName)
                .ToListAsync();
        }
    }
}