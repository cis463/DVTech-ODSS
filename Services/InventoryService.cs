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

        public async Task<bool> RestoreItemAsync(int id)
        {
            try
            {
                var item = await GetItemByIdAsync(id);
                if (item != null)
                {
                    item.IsArchived = false;
                    item.ArchivedDate = null;
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